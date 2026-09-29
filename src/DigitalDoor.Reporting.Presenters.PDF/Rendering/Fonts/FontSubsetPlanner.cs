namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal static class FontSubsetPlanner
{
    private const string PaginationCharacters = "0123456789";

    public static void ReplaceUsedTypefacesWithSubsets(ReportLayoutContent content, ReportTypefaceProvider typefaces)
    {
        Dictionary<SKTypeface, StringBuilder> usedTextByTypeface = new Dictionary<SKTypeface, StringBuilder>();
        foreach (ReportLayoutCell cell in content.GetAllCells())
        {
            string cellText = GetDrawnText(cell);
            if (!string.IsNullOrWhiteSpace(cellText))
            {
                SKTypeface typeface = typefaces.GetTypeface(cell.Format.FontDetails ?? new Font()).Typeface;
                if (!usedTextByTypeface.TryGetValue(typeface, out StringBuilder usedText))
                {
                    usedText = new StringBuilder();
                    usedTextByTypeface[typeface] = usedText;
                }
                usedText.Append(cellText);
            }
        }
        foreach (KeyValuePair<SKTypeface, StringBuilder> typefaceText in usedTextByTypeface)
        {
            SKTypeface subsetTypeface = CreateSubsetTypefaceOrNull(typefaceText.Key, typefaceText.Value.ToString());
            if (subsetTypeface is not null)
            {
                typefaces.ReplaceTypeface(typefaceText.Key, subsetTypeface);
            }
        }
    }

    private static SKTypeface CreateSubsetTypefaceOrNull(SKTypeface typeface, string usedText)
    {
        SKTypeface subsetTypeface = null;
        byte[] fontData = CustomFontLoader.ReadFontDataOrNull(typeface);
        if (fontData is not null)
        {
            byte[] subsetFontData = CreateSubsetFontDataOrNull(fontData, typeface.GetGlyphs(usedText));
            subsetTypeface = subsetFontData is null ? null : CustomFontLoader.LoadFromBytes(subsetFontData);
        }
        return subsetTypeface;
    }

    private static byte[] CreateSubsetFontDataOrNull(byte[] fontData, ushort[] usedGlyphIds)
    {
        byte[] subsetFontData;
        try
        {
            subsetFontData = TrueTypeSubsetter.CreateSubsetOrNull(fontData, usedGlyphIds);
        }
        catch (Exception exception) when (exception is ArgumentOutOfRangeException || exception is IndexOutOfRangeException || exception is OverflowException)
        {
            subsetFontData = null;
        }
        return subsetFontData;
    }

    private static string GetDrawnText(ReportLayoutCell cell)
    {
        string drawnText = null;
        if (cell.Pagination != ReportCellPagination.None)
        {
            drawnText = PaginationCharacters;
        }
        else if (cell.Value?.Kind == ReportCellValueKind.Text)
        {
            drawnText = cell.Value.Text;
        }
        return drawnText;
    }
}
