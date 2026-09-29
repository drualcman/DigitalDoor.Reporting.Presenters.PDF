using SkiaSharp;

namespace DigitalDoor.Reporting.Presenters.PDF.Samples;

public static class SampleImages
{
    public const string VectorBadgeSvg =
        "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" viewBox=\"0 0 24 24\">" +
        "<circle cx=\"12\" cy=\"12\" r=\"11\" fill=\"#1565C0\"/>" +
        "<path d=\"M6 12.5l4 4 8-9\" stroke=\"#FFFFFF\" stroke-width=\"2.5\" fill=\"none\" stroke-linecap=\"round\"/>" +
        "</svg>";

    public static byte[] CreatePngLogo()
    {
        using SKBitmap bitmap = new SKBitmap(240, 120);
        using SKCanvas canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Transparent);
        using SKPaint circlePaint = new SKPaint { Color = new SKColor(0xE6, 0x51, 0x00), IsAntialias = true };
        using SKPaint squarePaint = new SKPaint { Color = new SKColor(0x2E, 0x7D, 0x32, 0xCC), IsAntialias = true };
        canvas.DrawCircle(70, 60, 50, circlePaint);
        canvas.DrawRoundRect(new SKRect(110, 20, 220, 100), 16, 16, squarePaint);
        return Encode(bitmap, SKEncodedImageFormat.Png);
    }

    public static byte[] CreateJpegPhoto()
    {
        using SKBitmap bitmap = new SKBitmap(300, 200);
        using SKCanvas canvas = new SKCanvas(bitmap);
        using SKShader gradient = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(300, 200),
            new SKColor[] { new SKColor(0x42, 0xA5, 0xF5), new SKColor(0xAB, 0x47, 0xBC) }, SKShaderTileMode.Clamp);
        using SKPaint gradientPaint = new SKPaint { Shader = gradient };
        canvas.DrawRect(new SKRect(0, 0, 300, 200), gradientPaint);
        using SKPaint sunPaint = new SKPaint { Color = SKColors.Gold, IsAntialias = true };
        canvas.DrawCircle(230, 60, 35, sunPaint);
        return Encode(bitmap, SKEncodedImageFormat.Jpeg);
    }

    public static string CreateBase64Png()
    {
        return Convert.ToBase64String(CreatePngLogo());
    }

    private static byte[] Encode(SKBitmap bitmap, SKEncodedImageFormat format)
    {
        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData data = image.Encode(format, 90);
        return data.ToArray();
    }
}
