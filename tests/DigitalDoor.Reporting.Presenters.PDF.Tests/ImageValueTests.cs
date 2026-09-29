using System.Text.Json;
using DigitalDoor.Reporting.Presenters.PDF.Samples;

namespace DigitalDoor.Reporting.Presenters.PDF.Tests;

public class ImageValueTests
{
    private const string ImageColumn = "ImageColumn";

    public static TheoryData<string> RasterImageValueKinds => new TheoryData<string> { "bytes", "base64", "dataUri", "base64TextBytes" };

    [Theory]
    [MemberData(nameof(RasterImageValueKinds))]
    public async Task RasterImagesAreEmbedded(string valueKind)
    {
        byte[] png = SampleImages.CreatePngLogo();
        object value = valueKind switch
        {
            "bytes" => png,
            "base64" => Convert.ToBase64String(png),
            "dataUri" => $"data:image/png;base64,{Convert.ToBase64String(png)}",
            _ => System.Text.Encoding.UTF8.GetBytes(Convert.ToBase64String(png))
        };

        byte[] pdf = await GenerateWithImageAsync(value);

        Assert.True(PdfInspector.CountImages(pdf) >= 1);
    }

    [Fact]
    public async Task JsonElementValuesFromAWebAssemblyClientAreSupported()
    {
        Setup setup = CreateImageSetup();
        ReportViewModel viewModel = new ReportViewModel(setup, CreateImageData(SampleImages.CreatePngLogo()));
        string json = JsonSerializer.Serialize(viewModel);
        ReportViewModel deserializedViewModel = JsonSerializer.Deserialize<ReportViewModel>(json);

        byte[] pdf = await TestReportGenerator.GenerateFromViewModelAsync(deserializedViewModel);

        Assert.True(PdfInspector.CountImages(pdf) >= 1);
        Assert.Contains("Caption", PdfInspector.GetPageTexts(pdf)[0]);
    }

    [Fact]
    public async Task SvgImagesAreDrawnAsVectors()
    {
        byte[] pdf = await GenerateWithImageAsync(SampleImages.VectorBadgeSvg);

        Assert.Equal(0, PdfInspector.CountImages(pdf));
        Assert.True(pdf.Length < 60_000);
    }

    [Fact]
    public async Task InvalidImageBytesThrowADescriptiveException()
    {
        byte[] notAnImage = { 0x01, 0x02, 0x03, 0x04, 0x05 };

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() => GenerateWithImageAsync(notAnImage));

        Assert.Contains(ImageColumn, exception.Message);
    }

    private static async Task<byte[]> GenerateWithImageAsync(object imageValue)
    {
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(CreateImageSetup(), CreateImageData(imageValue));
        return report.Pdf;
    }

    private static Setup CreateImageSetup()
    {
        Setup setup = new Setup(PageSize.A4);
        setup.Body.Row = new Row(new Dimension(190, 50));
        setup.Body.AddColumn(ImageColumn, new Format(40, 20) { Position = new Kernel(5, 5) });
        setup.Body.AddColumn("Caption", TestSetups.CreateTextFormat(50, 5, 60, 8));
        return setup;
    }

    private static List<ColumnData> CreateImageData(object imageValue)
    {
        return new List<ColumnData>
        {
            new ColumnData(new Item(ImageColumn), imageValue, SectionType.Body),
            new ColumnData(new Item("Caption"), "Caption", SectionType.Body)
        };
    }
}
