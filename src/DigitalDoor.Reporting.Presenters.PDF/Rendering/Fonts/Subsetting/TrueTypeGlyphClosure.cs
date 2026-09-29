using System.Buffers.Binary;

namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts.Subsetting;

internal static class TrueTypeGlyphClosure
{
    private const int GlyphHeaderSize = 10;
    private const ushort ArgumentsAreWords = 0x0001;
    private const ushort HasSingleScale = 0x0008;
    private const ushort HasMoreComponents = 0x0020;
    private const ushort HasSeparateScales = 0x0040;
    private const ushort HasTwoByTwoMatrix = 0x0080;

    public static HashSet<int> Expand(ReadOnlySpan<byte> glyphTable, uint[] glyphOffsets, IEnumerable<ushort> usedGlyphIds)
    {
        int numberOfGlyphs = glyphOffsets.Length - 1;
        HashSet<int> keptGlyphs = new HashSet<int>();
        Stack<int> pendingGlyphs = new Stack<int>(usedGlyphIds.Select(glyphId => (int)glyphId));
        pendingGlyphs.Push(0);
        while (pendingGlyphs.Count > 0)
        {
            int glyphId = pendingGlyphs.Pop();
            if (glyphId < numberOfGlyphs && keptGlyphs.Add(glyphId))
            {
                foreach (int componentGlyphId in ReadComponentGlyphIds(glyphTable, glyphOffsets[glyphId], glyphOffsets[glyphId + 1]))
                {
                    pendingGlyphs.Push(componentGlyphId);
                }
            }
        }
        return keptGlyphs;
    }

    private static List<int> ReadComponentGlyphIds(ReadOnlySpan<byte> glyphTable, uint glyphStart, uint glyphEnd)
    {
        List<int> componentGlyphIds = new List<int>();
        bool isComposite = glyphEnd - glyphStart >= GlyphHeaderSize &&
            BinaryPrimitives.ReadInt16BigEndian(glyphTable.Slice((int)glyphStart, 2)) < 0;
        int position = (int)glyphStart + GlyphHeaderSize;
        bool hasMoreComponents = isComposite;
        while (hasMoreComponents && position + 4 <= glyphEnd)
        {
            ushort flags = BinaryPrimitives.ReadUInt16BigEndian(glyphTable.Slice(position, 2));
            componentGlyphIds.Add(BinaryPrimitives.ReadUInt16BigEndian(glyphTable.Slice(position + 2, 2)));
            position += 4 + GetArgumentsSize(flags) + GetTransformSize(flags);
            hasMoreComponents = (flags & HasMoreComponents) != 0;
        }
        return componentGlyphIds;
    }

    private static int GetArgumentsSize(ushort flags)
    {
        return (flags & ArgumentsAreWords) != 0 ? 4 : 2;
    }

    private static int GetTransformSize(ushort flags)
    {
        int transformSize = 0;
        if ((flags & HasSingleScale) != 0)
        {
            transformSize = 2;
        }
        else if ((flags & HasSeparateScales) != 0)
        {
            transformSize = 4;
        }
        else if ((flags & HasTwoByTwoMatrix) != 0)
        {
            transformSize = 8;
        }
        return transformSize;
    }
}
