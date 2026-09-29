using System.Buffers.Binary;

namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts.Subsetting;

internal static class TrueTypeSubsetter
{
    private const int HeadChecksumAdjustmentOffset = 8;
    private const int HeadIndexToLocationFormatOffset = 50;
    private const int MaximumProfileNumberOfGlyphsOffset = 4;
    private const int OS2EmbeddingTypeOffset = 8;
    private const ushort RestrictedLicenseEmbedding = 0x0002;
    private const ushort NoSubsettingEmbedding = 0x0100;
    private const int PostScriptHeaderSize = 32;
    private const uint PostScriptVersionWithoutGlyphNames = 0x00030000;
    private static readonly string[] CopiedTables = { "cmap", "cvt ", "fpgm", "gasp", "hhea", "hmtx", "maxp", "name", "OS/2", "prep" };

    public static byte[] CreateSubsetOrNull(byte[] fontData, IEnumerable<ushort> usedGlyphIds)
    {
        byte[] subsetFontData = null;
        TrueTypeTableDirectory directory = TrueTypeTableDirectory.Parse(fontData);
        if (directory.HasTrueTypeOutlines && AllowsSubsetting(directory))
        {
            ReadOnlySpan<byte> headTable = directory.GetTable("head");
            int numberOfGlyphs = BinaryPrimitives.ReadUInt16BigEndian(directory.GetTable("maxp").Slice(MaximumProfileNumberOfGlyphsOffset, 2));
            bool usesLongOffsets = BinaryPrimitives.ReadInt16BigEndian(headTable.Slice(HeadIndexToLocationFormatOffset, 2)) == 1;
            uint[] glyphOffsets = TrueTypeGlyphLocations.Read(directory.GetTable("loca"), numberOfGlyphs, usesLongOffsets);
            HashSet<int> keptGlyphs = TrueTypeGlyphClosure.Expand(directory.GetTable("glyf"), glyphOffsets, usedGlyphIds);
            TrueTypeGlyphTables glyphTables = TrueTypeGlyphTables.BuildKeepingGlyphs(directory.GetTable("glyf"), glyphOffsets, keptGlyphs);
            SortedDictionary<string, byte[]> subsetTables = new SortedDictionary<string, byte[]>(StringComparer.Ordinal)
            {
                { "glyf", glyphTables.GlyphData },
                { "loca", glyphTables.LongLocationData },
                { "head", CreateSubsetHeadTable(headTable) }
            };
            AddPostScriptTable(directory, subsetTables);
            foreach (string tag in CopiedTables.Where(directory.HasTable))
            {
                subsetTables[tag] = directory.GetTable(tag).ToArray();
            }
            subsetFontData = TrueTypeFontWriter.Write(subsetTables);
        }
        return subsetFontData;
    }

    private static bool AllowsSubsetting(TrueTypeTableDirectory directory)
    {
        ushort embeddingType = directory.HasTable("OS/2") ?
            BinaryPrimitives.ReadUInt16BigEndian(directory.GetTable("OS/2").Slice(OS2EmbeddingTypeOffset, 2)) : (ushort)0;
        return (embeddingType & (RestrictedLicenseEmbedding | NoSubsettingEmbedding)) == 0;
    }

    private static byte[] CreateSubsetHeadTable(ReadOnlySpan<byte> headTable)
    {
        byte[] subsetHeadTable = headTable.ToArray();
        BinaryPrimitives.WriteUInt32BigEndian(subsetHeadTable.AsSpan(HeadChecksumAdjustmentOffset, 4), 0);
        BinaryPrimitives.WriteInt16BigEndian(subsetHeadTable.AsSpan(HeadIndexToLocationFormatOffset, 2), 1);
        return subsetHeadTable;
    }

    private static void AddPostScriptTable(TrueTypeTableDirectory directory, SortedDictionary<string, byte[]> subsetTables)
    {
        if (directory.HasTable("post") && directory.GetTable("post").Length >= PostScriptHeaderSize)
        {
            byte[] postScriptTable = directory.GetTable("post").Slice(0, PostScriptHeaderSize).ToArray();
            BinaryPrimitives.WriteUInt32BigEndian(postScriptTable.AsSpan(0, 4), PostScriptVersionWithoutGlyphNames);
            subsetTables["post"] = postScriptTable;
        }
    }
}
