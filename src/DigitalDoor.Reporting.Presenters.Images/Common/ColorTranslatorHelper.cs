namespace DigitalDoor.Reporting.Presenters.Images.Common;
internal static class ColorTranslatorHelper
{
    internal static SKColor ConvertToSKColor(string color)
    {
        color = ColorTranslator.ConvertToHexColor(color);
        return ConvertHexToSKColor(CssColors.ColorToHex(color));
    }

    private static SKColor ConvertHexToSKColor(string hex)
    {
        if (hex.Length == 4)
        {
            hex = $"#{hex[1]}{hex[1]}{hex[2]}{hex[2]}{hex[3]}{hex[3]}";
        }
        return SKColor.Parse(hex);
    }
}
