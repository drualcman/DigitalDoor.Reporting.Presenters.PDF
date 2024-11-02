using DigitalDoor.Reporting.Entities.ValueObjects;
using SkiaSharp;

internal static class FontStyleHelper
{
    internal static SKFontStyle ConvertToSKFontStyle(FontStyle fontStyle)
    {
        SKFontStyleWeight weight = fontStyle.IsBold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
        SKFontStyleWidth width = SKFontStyleWidth.Normal;
        SKFontStyleSlant slant = fontStyle.Italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
        return new SKFontStyle(weight, width, slant);
    }
}
