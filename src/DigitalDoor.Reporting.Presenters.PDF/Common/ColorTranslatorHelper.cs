namespace DigitalDoor.Reporting.Presenters.PDF.Common;

internal static class ColorTranslatorHelper
{
    private const string TransparentColourName = "transparent";

    public static bool IsTransparent(string colour)
    {
        return string.IsNullOrWhiteSpace(colour) ||
            colour.Trim().Equals(TransparentColourName, StringComparison.OrdinalIgnoreCase);
    }

    public static SKColor ConvertToSKColor(string colour)
    {
        SKColor result = SKColors.Transparent;
        if (!IsTransparent(colour))
        {
            RgbColors rgbColour = RgbColors.FromHex(ColorTranslator.ConvertToHexColor(colour.Trim()));
            result = new SKColor((byte)rgbColour.R, (byte)rgbColour.G, (byte)rgbColour.B, (byte)rgbColour.A);
        }
        return result;
    }

    public static byte ConvertOpacityToAlpha(float opacity)
    {
        float clampedOpacity = Math.Clamp(opacity, 0f, 1f);
        return (byte)Math.Round(clampedOpacity * byte.MaxValue);
    }
}
