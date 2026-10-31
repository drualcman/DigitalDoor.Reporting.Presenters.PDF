namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal static class SvgFontFamilyName
{
    private static readonly Dictionary<string, string> GenericFamilies = new(StringComparer.OrdinalIgnoreCase)
    {
        { "sans-serif", "Arial" },
        { "serif", "Times New Roman" },
        { "monospace", "Courier New" },
        { "system-ui", "Arial" }
    };

    public static string ToReportFontName(string svgFontFamilies)
    {
        string firstFamily = (svgFontFamilies ?? string.Empty)
            .Split(',')
            .Select(family => family.Trim().Trim('\'', '"').Trim())
            .FirstOrDefault(family => family.Length > 0);
        string result = FontFamilyAliases.DefaultFontName;
        if (firstFamily is not null)
        {
            result = GenericFamilies.TryGetValue(firstFamily, out string standardFamily) ? standardFamily : firstFamily;
        }
        return result;
    }

    public static Font CreateFont(string svgFontFamilies, bool bold, bool italic)
    {
        return new Font(ToReportFontName(svgFontFamilies))
        {
            FontStyle = new FontStyle(bold ? SKFontStyle.Bold.Weight : SKFontStyle.Normal.Weight, italic)
        };
    }
}
