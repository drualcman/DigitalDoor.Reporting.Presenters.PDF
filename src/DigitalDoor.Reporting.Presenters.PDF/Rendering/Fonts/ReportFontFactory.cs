namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal static class ReportFontFactory
{
    private const float SyntheticItalicSkew = -0.25f;

    public static SKFont CreateFont(ResolvedTypeface resolvedTypeface, Font fontDetails)
    {
        SKFont font = new SKFont(resolvedTypeface.Typeface, GetFontSizeInPoints(fontDetails))
        {
            Embolden = resolvedTypeface.RequiresSyntheticBold,
            SkewX = resolvedTypeface.RequiresSyntheticItalic ? SyntheticItalicSkew : 0f,
            Hinting = SKFontHinting.None,
            Subpixel = true,
            LinearMetrics = true,
            Edging = SKFontEdging.Antialias
        };
        return font;
    }

    public static float GetFontSizeInPoints(Font fontDetails)
    {
        double fontSize = fontDetails.ColorSize?.Width ?? 0;
        return (float)Math.Max(0, fontSize);
    }
}
