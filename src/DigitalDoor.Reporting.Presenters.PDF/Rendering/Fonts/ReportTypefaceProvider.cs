namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal sealed class ReportTypefaceProvider : IDisposable
{
    private readonly IReportFont ReportFont;
    private readonly Dictionary<string, SKTypeface> CustomTypefacesByFontName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, ResolvedTypeface> ResolvedTypefacesByKey = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<SKTypeface> OwnedTypefaces = new();

    public ReportTypefaceProvider(IReportFont reportFont)
    {
        ReportFont = reportFont;
    }

    public ResolvedTypeface GetTypeface(Font fontDetails)
    {
        string fontName = string.IsNullOrWhiteSpace(fontDetails.FontName) ? FontFamilyAliases.DefaultFontName : fontDetails.FontName.Trim();
        FontStyle fontStyle = fontDetails.FontStyle ?? new FontStyle();
        bool bold = FontStyleHelper.IsBoldWeight(fontStyle.Bold) || FontFamilyAliases.ImpliesBold(fontName);
        bool italic = fontStyle.Italic || FontFamilyAliases.ImpliesItalic(fontName);
        string cacheKey = $"{fontName}|{bold}|{italic}";
        if (!ResolvedTypefacesByKey.TryGetValue(cacheKey, out ResolvedTypeface resolvedTypeface))
        {
            resolvedTypeface = new ResolvedTypeface(ResolveTypeface(fontName, bold, italic), bold, italic);
            ResolvedTypefacesByKey[cacheKey] = resolvedTypeface;
        }
        return resolvedTypeface;
    }

    public void ReplaceTypeface(SKTypeface originalTypeface, SKTypeface equivalentTypeface)
    {
        AddOwnedTypeface(equivalentTypeface);
        foreach (ResolvedTypeface resolvedTypeface in ResolvedTypefacesByKey.Values.Where(resolved => resolved.Typeface == originalTypeface))
        {
            resolvedTypeface.ReplaceTypeface(equivalentTypeface);
        }
    }

    private SKTypeface ResolveTypeface(string fontName, bool bold, bool italic)
    {
        SKTypeface typeface = null;
        if (ReportFont is not null && !FontFamilyAliases.IsStandardFontName(fontName))
        {
            typeface = GetCustomTypeface(fontName);
        }
        if (typeface is null)
        {
            SKFontStyle requestedStyle = FontStyleHelper.ConvertToSKFontStyle(bold ? SKFontStyle.Bold.Weight : SKFontStyle.Normal.Weight, italic);
            typeface = SystemTypefaceMatcher.MatchFirstInstalledFamily(FontFamilyAliases.GetCandidateFamilies(fontName), requestedStyle);
            AddOwnedTypeface(typeface);
        }
        return typeface ?? EmbeddedFallbackTypefaces.GetTypeface(bold, italic);
    }

    private SKTypeface GetCustomTypeface(string fontName)
    {
        if (!CustomTypefacesByFontName.TryGetValue(fontName, out SKTypeface customTypeface))
        {
            customTypeface = CustomFontLoader.LoadOrNull(ReportFont, fontName);
            AddOwnedTypeface(customTypeface);
            CustomTypefacesByFontName[fontName] = customTypeface;
        }
        return customTypeface;
    }

    private void AddOwnedTypeface(SKTypeface typeface)
    {
        if (typeface is not null)
        {
            OwnedTypefaces.Add(typeface);
        }
    }

    public void Dispose()
    {
        foreach (SKTypeface ownedTypeface in OwnedTypefaces)
        {
            ownedTypeface.Dispose();
        }
        OwnedTypefaces.Clear();
    }
}
