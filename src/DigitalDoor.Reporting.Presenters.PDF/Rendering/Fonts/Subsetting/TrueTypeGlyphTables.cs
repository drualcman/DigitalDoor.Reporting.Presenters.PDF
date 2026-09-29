using System.Buffers.Binary;

namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts.Subsetting;

internal sealed class TrueTypeGlyphTables
{
    private const int GlyphDataAlignment = 4;

    public byte[] GlyphData { get; }
    public byte[] LongLocationData { get; }

    private TrueTypeGlyphTables(byte[] glyphData, byte[] longLocationData)
    {
        GlyphData = glyphData;
        LongLocationData = longLocationData;
    }

    public static TrueTypeGlyphTables BuildKeepingGlyphs(ReadOnlySpan<byte> glyphTable, uint[] glyphOffsets, HashSet<int> keptGlyphs)
    {
        int numberOfGlyphs = glyphOffsets.Length - 1;
        byte[] longLocationData = new byte[(numberOfGlyphs + 1) * 4];
        using MemoryStream glyphDataStream = new MemoryStream();
        for (int glyphId = 0; glyphId < numberOfGlyphs; glyphId++)
        {
            BinaryPrimitives.WriteUInt32BigEndian(longLocationData.AsSpan(glyphId * 4, 4), (uint)glyphDataStream.Length);
            uint glyphStart = glyphOffsets[glyphId];
            uint glyphEnd = glyphOffsets[glyphId + 1];
            if (keptGlyphs.Contains(glyphId) && glyphEnd > glyphStart)
            {
                glyphDataStream.Write(glyphTable.Slice((int)glyphStart, (int)(glyphEnd - glyphStart)));
                WriteAlignmentPadding(glyphDataStream);
            }
        }
        BinaryPrimitives.WriteUInt32BigEndian(longLocationData.AsSpan(numberOfGlyphs * 4, 4), (uint)glyphDataStream.Length);
        return new TrueTypeGlyphTables(glyphDataStream.ToArray(), longLocationData);
    }

    private static void WriteAlignmentPadding(MemoryStream glyphDataStream)
    {
        while (glyphDataStream.Length % GlyphDataAlignment != 0)
        {
            glyphDataStream.WriteByte(0);
        }
    }
}
