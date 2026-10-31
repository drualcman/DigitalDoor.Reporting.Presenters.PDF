using Svg.Skia.TypefaceProviders;

namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal sealed class ReportSvgTypefaceProvider : ITypefaceProvider
{
    private readonly ReportTypefaceProvider Typefaces;

    public ReportSvgTypefaceProvider(ReportTypefaceProvider typefaces)
    {
        Typefaces = typefaces;
    }

    public SKTypeface FromFamilyName(string fontFamily, SKFontStyleWeight fontWeight, SKFontStyleWidth fontWidth, SKFontStyleSlant fontStyle)
    {
        bool bold = FontStyleHelper.IsBoldWeight((int)fontWeight);
        bool italic = fontStyle != SKFontStyleSlant.Upright;
        return Typefaces.GetTypeface(SvgFontFamilyName.CreateFont(fontFamily, bold, italic)).Typeface;
    }
}
