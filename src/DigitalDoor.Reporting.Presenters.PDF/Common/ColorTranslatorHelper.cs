namespace DigitalDoor.Reporting.Presenters.PDF.Common;
internal static class ColorTranslatorHelper
{
    public static RgbColors GetColor(string color)
    {
        return RgbColors.FromHex(ColorTranslator.ConvertToHexColor(color));
    }
}
