namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal static class SystemTypefaceMatcher
{
    public static SKTypeface MatchFirstInstalledFamily(IReadOnlyList<string> candidateFamilies, SKFontStyle fontStyle)
    {
        SKTypeface matchedTypeface = null;
        int candidateIndex = 0;
        while (candidateIndex < candidateFamilies.Count && matchedTypeface is null)
        {
            matchedTypeface = MatchInstalledFamily(candidateFamilies[candidateIndex], fontStyle);
            candidateIndex++;
        }
        return matchedTypeface;
    }

    private static SKTypeface MatchInstalledFamily(string familyName, SKFontStyle fontStyle)
    {
        SKTypeface typeface = SKFontManager.Default.MatchFamily(familyName, fontStyle);
        if (typeface is not null && !typeface.FamilyName.Equals(familyName, StringComparison.OrdinalIgnoreCase))
        {
            typeface.Dispose();
            typeface = null;
        }
        return typeface;
    }
}
