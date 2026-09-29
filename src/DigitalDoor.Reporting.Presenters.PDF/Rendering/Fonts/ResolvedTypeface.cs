namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal sealed class ResolvedTypeface
{
    public SKTypeface Typeface { get; private set; }
    public bool RequiresSyntheticBold { get; }
    public bool RequiresSyntheticItalic { get; }

    public ResolvedTypeface(SKTypeface typeface, bool requestedBold, bool requestedItalic)
    {
        Typeface = typeface;
        RequiresSyntheticBold = requestedBold && !FontStyleHelper.IsBoldWeight(typeface.FontWeight);
        RequiresSyntheticItalic = requestedItalic && typeface.FontSlant == SKFontStyleSlant.Upright;
    }

    public void ReplaceTypeface(SKTypeface equivalentTypeface)
    {
        Typeface = equivalentTypeface;
    }
}
