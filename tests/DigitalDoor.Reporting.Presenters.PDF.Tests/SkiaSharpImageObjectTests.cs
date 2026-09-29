using SkiaSharp;
using DigitalDoor.Reporting.Presenters.PDF.Samples;

namespace DigitalDoor.Reporting.Presenters.PDF.Tests;

public class SkiaSharpImageObjectTests
{
    private const string ImageColumn = "ImageColumn";

    [Fact]
    public async Task SKImageValuesAreEmbedded()
    {
        using SKImage image = SKImage.FromEncodedData(SampleImages.CreatePngLogo());

        byte[] pdf = await GenerateWithImageAsync(image);

        Assert.True(PdfInspector.CountImages(pdf) >= 1);
    }

    [Fact]
    public async Task SKBitmapValuesAreEmbedded()
    {
        using SKBitmap bitmap = SKBitmap.Decode(SampleImages.CreatePngLogo());

        byte[] pdf = await GenerateWithImageAsync(bitmap);

        Assert.True(PdfInspector.CountImages(pdf) >= 1);
    }

    [Fact]
    public async Task SKPictureValuesAreDrawnAsVectors()
    {
        using SKPicture picture = CreateCirclePicture();

        byte[] pdf = await GenerateWithImageAsync(picture);

        Assert.Equal(0, PdfInspector.CountImages(pdf));
        Assert.Contains("Caption", PdfInspector.GetPageTexts(pdf)[0]);
    }

    private static SKPicture CreateCirclePicture()
    {
        using SKPictureRecorder recorder = new SKPictureRecorder();
        SKCanvas canvas = recorder.BeginRecording(new SKRect(0, 0, 100, 100));
        using SKPaint paint = new SKPaint { Color = SKColors.OrangeRed, IsAntialias = true };
        canvas.DrawCircle(50, 50, 40, paint);
        return recorder.EndRecording();
    }

    private static async Task<byte[]> GenerateWithImageAsync(object imageValue)
    {
        Setup setup = new Setup(PageSize.A4);
        setup.Body.Row = new Row(new Dimension(190, 50));
        setup.Body.AddColumn(ImageColumn, new Format(40, 20) { Position = new Kernel(5, 5) });
        setup.Body.AddColumn("Caption", TestSetups.CreateTextFormat(50, 5, 60, 8));
        List<ColumnData> data = new List<ColumnData>
        {
            new ColumnData(new Item(ImageColumn), imageValue, SectionType.Body),
            new ColumnData(new Item("Caption"), "Caption", SectionType.Body)
        };
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(setup, data);
        return report.Pdf;
    }
}
