# DigitalDoor.Reporting.Presenters.PDF

PDF presenter for DigitalDoor.Reporting based on SkiaSharp. The package documentation is in
[src/DigitalDoor.Reporting.Presenters.PDF/README.md](src/DigitalDoor.Reporting.Presenters.PDF/README.md).

## Repository structure
- `src/DigitalDoor.Reporting.Presenters.PDF`: the NuGet package.
- `tests/DigitalDoor.Reporting.Presenters.PDF.Tests`: pagination, geometry, images, fonts, font subsetting, public API and concurrency tests.
- `samples/Shared`: a report with several pages, header and footer with pagination, images, SVG and long text.
- `samples/DigitalDoor.Reporting.Presenters.PDF.Samples.Skia`: generates the sample report with this repository.
- `samples/DigitalDoor.Reporting.Presenters.PDF.Samples.Legacy`: generates the same report with the published 1.17.71 package (iText, AGPL) to compare. It is not part of the solution and it is not distributed.
- `samples/DigitalDoor.Reporting.Presenters.PDF.Samples.Wasm`: generates the sample report inside Blazor WebAssembly.
- `samples/DigitalDoor.Reporting.Presenters.PDF.Samples.Rasterizer`: converts the PDF pages to PNG to compare them.

## Compare with the previous version
```bash
dotnet run --project samples/DigitalDoor.Reporting.Presenters.PDF.Samples.Legacy -- samples/output
dotnet run --project samples/DigitalDoor.Reporting.Presenters.PDF.Samples.Skia -- samples/output
dotnet run --project samples/DigitalDoor.Reporting.Presenters.PDF.Samples.Rasterizer -- samples/output/png 100 samples/output/itext-portrait.pdf samples/output/skia-portrait.pdf
```

## Run the tests
```bash
dotnet test tests/DigitalDoor.Reporting.Presenters.PDF.Tests
```

## Run the sample in Linux without fonts
```bash
dotnet publish samples/DigitalDoor.Reporting.Presenters.PDF.Samples.Skia -c Release -r linux-musl-x64 --self-contained false -o samples/output/linux
docker run --rm -v "$PWD/samples/output/linux:/app" -w /app mcr.microsoft.com/dotnet/runtime:10.0-alpine dotnet DigitalDoor.Reporting.Presenters.PDF.Samples.Skia.dll /app/result
```
