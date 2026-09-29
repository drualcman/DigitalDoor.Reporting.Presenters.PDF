namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts;

internal static class CustomFontLoader
{
    public static SKTypeface LoadOrNull(IReportFont reportFont, string fontName)
    {
        byte[] fontBytes;
        try
        {
            fontBytes = reportFont.GetFontBytesArray(fontName);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"IReportFont failed to provide the font '{fontName}'.", exception);
        }
        SKTypeface customTypeface = null;
        if (fontBytes is not null && fontBytes.Length > 0)
        {
            customTypeface = LoadFromBytes(fontBytes) ??
                throw new InvalidOperationException($"The font '{fontName}' provided by IReportFont is not a valid TrueType or OpenType font.");
        }
        return customTypeface;
    }

    public static SKTypeface LoadFromBytes(byte[] fontBytes)
    {
        using SKData fontData = SKData.CreateCopy(fontBytes);
        return SKTypeface.FromData(fontData);
    }

    public static byte[] ReadFontDataOrNull(SKTypeface typeface)
    {
        byte[] fontData = null;
        using SKStreamAsset fontStream = typeface.OpenStream(out int collectionIndex);
        if (fontStream is not null && collectionIndex == 0 && fontStream.Length > 0)
        {
            fontData = new byte[fontStream.Length];
            int bytesRead = fontStream.Read(fontData, fontData.Length);
            fontData = bytesRead == fontData.Length ? fontData : null;
        }
        return fontData;
    }
}
