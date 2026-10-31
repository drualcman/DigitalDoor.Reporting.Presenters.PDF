# DigitalDoor.Reporting.Presenters.PDF

Generate PDF reports from a `ReportViewModel` of [DigitalDoor.Reporting](https://www.nuget.org/packages/DigitalDoor.Reporting).
The PDF is drawn with [SkiaSharp](https://github.com/mono/SkiaSharp) and works in ASP.NET Core, Blazor Server,
Blazor WebAssembly, MVC, console apps and Linux containers.

## Register the services
```csharp
builder.Services.AddReportingPresenterPdfServices();
```

## Create a PDF
Build the `ReportViewModel` with `IReportsOutputPort` and `IReportsPresenter`, then generate the bytes with `IReportAsBytes`.
```csharp
class InvoiceReport
{
    readonly IReportsOutputPort ReportsOutputPort;
    readonly IReportsPresenter ReportsPresenter;
    readonly IReportAsBytes ReportAsBytes;

    public InvoiceReport(IReportsOutputPort reportsOutputPort, IReportsPresenter reportsPresenter, IReportAsBytes reportAsBytes)
    {
        ReportsOutputPort = reportsOutputPort;
        ReportsPresenter = reportsPresenter;
        ReportAsBytes = reportAsBytes;
    }

    public async Task<byte[]> CreatePdfAsync(Setup setup, List<ColumnData> data)
    {
        await ReportsOutputPort.Handle(setup, data);
        ReportViewModel reportViewModel = ReportsPresenter.Content;
        return await ReportAsBytes.GenerateReport(reportViewModel);
    }
}
```
`IReportAsBytes<PDFReportPresenter>`, `IPDFReportOutputPort` and `IPDFReportPresenter` keep working as in previous versions.

## Same result in the PDF and in the HTML view
The PDF and the HTML created by `ReportHtmlGenerator` (used by the Blazor `ReportView` component) share the same layout code
from `DigitalDoor.Reporting.Entities.Layout`: page and section geometry, cell boxes, rows, columns, pagination, z-order and
value types. Both use the same line height (1.15), line breaking and clipping rules, so the preview in Blazor shows what the PDF will contain.
Browsers draw very thin `double` borders as solid and the PDF does the same.

## What is rendered
- Page size in real ISO points (72 per inch), orientation, page margins and paddings.
- Header, body and footer stacked from the top of the page. Header and footer are repeated on every page.
- `CurrentPage` and `TotalPages` in the header, body or footer (`Section.AddCurentPage()` and `Section.AddTotalPages()`).
- Body with several columns (`ColumnsNumber`, `ColumnsSpace`) and row borders.
- Borders with style (`solid`, `dashed`, `dotted`, `double`, `groove`, `ridge`, `inset`, `outset`, `hidden`), width, colour and corner radius.
- Backgrounds, opacity (`FontDetails.ColorSize.Opacity`), rotation around the centre of the cell (`Angle`) and z-order (`Foreground`).
- Fonts with name, size in points, colour, weight and italic. Text alignment (including justify) and decoration (underline, line-through, overline).
- Text is wrapped inside the width of the cell and the lines that do not fit in the height are not drawn.
- Images as `byte[]`, base64 `string`, data URI, `JsonElement` (for example when the view model comes from a Blazor WebAssembly client), `SKImage`, `SKBitmap`, `SKPicture` and SVG. SVG images are drawn as vectors, and their text uses the same fonts as the rest of the report (see Fonts), so charts and labels inside an SVG are also drawn in Linux, Docker and Blazor WebAssembly.

## Fonts
The font of each cell is resolved in this order:
1. Your `IReportFont` implementation, for font names that are not standard PDF fonts (Arial, Helvetica, Times, Courier).
2. Fonts installed in the operating system.
3. The embedded **Liberation Sans** font, metric-compatible with Arial and Helvetica. It is always available, also in Docker and in the browser.

Fonts are embedded in the PDF with only the glyphs used by the report, so the PDF can be opened on any device and supports any Unicode character included in the font.
Bold and italic are simulated when the font does not have that style.

```csharp
public class ReportFont : IReportFont
{
    public byte[] GetFontBytesArray(string fontName) =>
        File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fonts", $"{fontName}.ttf"));
}

builder.Services.AddSingleton<IReportFont, ReportFont>();
```
Return `null` when you do not have the font to use the fallback font. Exceptions thrown by `IReportFont`, invalid font files and invalid images are not hidden anymore: they throw an `InvalidOperationException` with the name of the font or the column.

## Linux and Docker
The package includes `SkiaSharp.NativeAssets.Linux.NoDependencies` (x64, ARM64 and musl builds), so it works in Linux containers
**without installing fontconfig or fonts**. It has been tested in the `mcr.microsoft.com/dotnet/runtime` Debian and Alpine images;
the same native builds are used by Amazon Linux, AWS Lambda and ARM64 (Graviton).
Without fontconfig only the embedded font and the fonts from `IReportFont` are available.

If you prefer to use the fonts installed in the container, replace the native assets in your application:
```xml
<PackageReference Include="SkiaSharp.NativeAssets.Linux" Version="4.153.0" />
<PackageReference Include="SkiaSharp.NativeAssets.Linux.NoDependencies" Version="4.153.0" ExcludeAssets="all" />
```
```dockerfile
# Debian or Ubuntu images
RUN apt-get update && apt-get install -y --no-install-recommends libfontconfig1 fonts-liberation && rm -rf /var/lib/apt/lists/*
# Alpine images
RUN apk add --no-cache fontconfig ttf-liberation
```

## Blazor WebAssembly
The PDF can be generated in the browser. The package includes `SkiaSharp.NativeAssets.WebAssembly`, which links the native
SkiaSharp library into the application. Install the WebAssembly build tools once in the machine or the build server:
```bash
dotnet workload install wasm-tools
```
The first publish takes more time because the native code is linked, and the application download grows by about 3 MB (compressed).

## Migrating from 1.17 (iText)
The public API is the same, but the rendering engine changed and some results are different:
- The license is MIT. iText and its AGPL license are no longer used.
- The page uses real ISO sizes. The previous version created pages about 4% larger than the paper size.
- Page margins and paddings are applied, and the sections are stacked from the top of the page like the HTML view. Before, the content was anchored to the bottom of the page and the margins were ignored.
- The font size is in points, the same as the HTML view (before it was about 26% smaller).
- Text is placed from the top of the cell, and rotated cells turn around their centre.
- `CurrentPage` and `TotalPages` now work in the header and the total is correct.
- `TextDecoration` and the page background are now rendered.
- Borders with `BorderStyle.none` and a width are still drawn as solid borders, as before.

## License
This project is licensed under the MIT License. It uses SkiaSharp (MIT), Svg.Skia (MIT) and embeds the Liberation Sans fonts (SIL Open Font License 1.1). See `THIRD-PARTY-NOTICES.txt`.

# Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

- Fork the repository
- Create your feature branch (git checkout -b feature/AmazingFeature)
- Commit your changes (git commit -m 'Add some AmazingFeature')
- Push to the branch (git push origin feature/AmazingFeature)
- Open a Pull Request

# Made with love by DrUalcman

If you find this component useful, please consider giving it a star on GitHub!
