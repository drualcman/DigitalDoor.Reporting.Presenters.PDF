using DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts.Subsetting;
using SkiaSharp;

namespace DigitalDoor.Reporting.Presenters.PDF.Tests;

public class TrueTypeSubsetterTests
{
    private const string UsedText = "Hola, informe áéíóú €";

    [Fact]
    public void SubsetKeepsGlyphIdentifiersAndMetrics()
    {
        byte[] fontData = TestFonts.ReadFontFile(TestFonts.RegularFontFile);
        using SKTypeface originalTypeface = SKTypeface.FromData(SKData.CreateCopy(fontData));
        ushort[] usedGlyphs = originalTypeface.GetGlyphs(UsedText);

        byte[] subsetData = TrueTypeSubsetter.CreateSubsetOrNull(fontData, usedGlyphs);
        using SKTypeface subsetTypeface = SKTypeface.FromData(SKData.CreateCopy(subsetData));
        using SKFont originalFont = new SKFont(originalTypeface, 12);
        using SKFont subsetFont = new SKFont(subsetTypeface, 12);

        Assert.NotNull(subsetTypeface);
        Assert.True(subsetData.Length < fontData.Length / 3);
        Assert.Equal(usedGlyphs, subsetTypeface.GetGlyphs(UsedText));
        Assert.Equal(originalTypeface.GlyphCount, subsetTypeface.GlyphCount);
        Assert.Equal(originalFont.MeasureText(UsedText), subsetFont.MeasureText(UsedText), 3);
        Assert.Equal(originalTypeface.FamilyName, subsetTypeface.FamilyName);
    }

    [Fact]
    public void SubsetKeepsTheOutlinesOfUsedGlyphs()
    {
        byte[] fontData = TestFonts.ReadFontFile(TestFonts.RegularFontFile);
        using SKTypeface originalTypeface = SKTypeface.FromData(SKData.CreateCopy(fontData));
        byte[] subsetData = TrueTypeSubsetter.CreateSubsetOrNull(fontData, originalTypeface.GetGlyphs("Aé"));
        using SKTypeface subsetTypeface = SKTypeface.FromData(SKData.CreateCopy(subsetData));
        using SKFont subsetFont = new SKFont(subsetTypeface, 12);

        using SKPath usedGlyphPath = subsetFont.GetGlyphPath(subsetTypeface.GetGlyphs("é")[0]);
        using SKPath removedGlyphPath = subsetFont.GetGlyphPath(subsetTypeface.GetGlyphs("Z")[0]);

        Assert.False(usedGlyphPath.IsEmpty);
        Assert.True(removedGlyphPath is null || removedGlyphPath.IsEmpty);
    }

    [Fact]
    public void NonTrueTypeDataIsNotSubset()
    {
        byte[] notAFont = new byte[64];

        Assert.Null(TrueTypeSubsetter.CreateSubsetOrNull(notAFont, new ushort[] { 1 }));
    }
}
