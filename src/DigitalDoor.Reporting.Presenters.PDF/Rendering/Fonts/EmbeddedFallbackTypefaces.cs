using System.Reflection;

namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal static class EmbeddedFallbackTypefaces
{
    private const string ResourceNamePrefix = "DigitalDoor.Reporting.Presenters.PDF.Fonts.";

    private static readonly Lazy<SKTypeface> RegularTypeface = new(() => LoadTypeface("LiberationSans-Regular.ttf"));
    private static readonly Lazy<SKTypeface> BoldTypeface = new(() => LoadTypeface("LiberationSans-Bold.ttf"));
    private static readonly Lazy<SKTypeface> ItalicTypeface = new(() => LoadTypeface("LiberationSans-Italic.ttf"));
    private static readonly Lazy<SKTypeface> BoldItalicTypeface = new(() => LoadTypeface("LiberationSans-BoldItalic.ttf"));

    public static SKTypeface GetTypeface(bool bold, bool italic)
    {
        Lazy<SKTypeface> selectedTypeface = RegularTypeface;
        if (bold && italic)
        {
            selectedTypeface = BoldItalicTypeface;
        }
        else if (bold)
        {
            selectedTypeface = BoldTypeface;
        }
        else if (italic)
        {
            selectedTypeface = ItalicTypeface;
        }
        return selectedTypeface.Value;
    }

    private static SKTypeface LoadTypeface(string fontFileName)
    {
        Assembly resourcesAssembly = typeof(EmbeddedFallbackTypefaces).Assembly;
        using Stream fontStream = resourcesAssembly.GetManifestResourceStream(ResourceNamePrefix + fontFileName) ??
            throw new InvalidOperationException($"The embedded fallback font '{fontFileName}' was not found.");
        using SKData fontData = SKData.Create(fontStream);
        return SKTypeface.FromData(fontData) ??
            throw new InvalidOperationException($"The embedded fallback font '{fontFileName}' could not be loaded by SkiaSharp.");
    }
}
