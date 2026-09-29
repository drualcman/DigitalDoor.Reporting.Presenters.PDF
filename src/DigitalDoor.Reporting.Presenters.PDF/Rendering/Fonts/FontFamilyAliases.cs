namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal static class FontFamilyAliases
{
    public const string DefaultFontName = "Arial";

    private static readonly string[] SansSerifFamilies = { "Arial", "Helvetica", "Liberation Sans", "Arimo" };
    private static readonly string[] SerifFamilies = { "Times New Roman", "Times", "Liberation Serif", "Tinos" };
    private static readonly string[] MonospaceFamilies = { "Courier New", "Courier", "Liberation Mono", "Cousine" };

    private static readonly Dictionary<string, string[]> StandardFamilies = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Arial", SansSerifFamilies },
        { "Helvetica", SansSerifFamilies },
        { "Times", SerifFamilies },
        { "Times New Roman", SerifFamilies },
        { "Courier", MonospaceFamilies },
        { "Courier New", MonospaceFamilies }
    };

    public static bool IsStandardFontName(string fontName)
    {
        return StandardFamilies.ContainsKey(GetBaseFamilyName(fontName));
    }

    public static IReadOnlyList<string> GetCandidateFamilies(string fontName)
    {
        IReadOnlyList<string> candidates = new[] { fontName };
        if (StandardFamilies.TryGetValue(GetBaseFamilyName(fontName), out string[] standardCandidates))
        {
            candidates = standardCandidates;
        }
        return candidates;
    }

    public static bool ImpliesBold(string fontName)
    {
        return IsStandardFontName(fontName) && fontName.IndexOf("Bold", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool ImpliesItalic(string fontName)
    {
        return IsStandardFontName(fontName) &&
            (fontName.IndexOf("Italic", StringComparison.OrdinalIgnoreCase) >= 0 ||
             fontName.IndexOf("Oblique", StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static string GetBaseFamilyName(string fontName)
    {
        string baseFamilyName = fontName.Trim();
        int styleSeparatorIndex = baseFamilyName.IndexOf('-');
        if (styleSeparatorIndex > 0)
        {
            baseFamilyName = baseFamilyName.Substring(0, styleSeparatorIndex);
        }
        return baseFamilyName;
    }
}
