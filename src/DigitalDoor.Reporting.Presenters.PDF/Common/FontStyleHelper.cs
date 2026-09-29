namespace DigitalDoor.Reporting.Presenters.PDF.Common;

internal static class FontStyleHelper
{
    public const int BoldWeightThreshold = 600;
    private const int MinimumFontWeight = 100;
    private const int MaximumFontWeight = 1000;

    public static SKFontStyle ConvertToSKFontStyle(int fontWeight, bool italic)
    {
        int clampedWeight = Math.Clamp(fontWeight, MinimumFontWeight, MaximumFontWeight);
        SKFontStyleSlant slant = italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
        return new SKFontStyle(clampedWeight, (int)SKFontStyleWidth.Normal, slant);
    }

    public static bool IsBoldWeight(int fontWeight)
    {
        return fontWeight >= BoldWeightThreshold;
    }
}
