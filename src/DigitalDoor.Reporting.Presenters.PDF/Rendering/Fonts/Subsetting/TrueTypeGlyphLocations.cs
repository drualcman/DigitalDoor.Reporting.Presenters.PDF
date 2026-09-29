using System.Buffers.Binary;

namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts.Subsetting;

internal static class TrueTypeGlyphLocations
{
    public static uint[] Read(ReadOnlySpan<byte> locationTable, int numberOfGlyphs, bool usesLongOffsets)
    {
        uint[] glyphOffsets = new uint[numberOfGlyphs + 1];
        for (int glyphIndex = 0; glyphIndex <= numberOfGlyphs; glyphIndex++)
        {
            glyphOffsets[glyphIndex] = usesLongOffsets ?
                BinaryPrimitives.ReadUInt32BigEndian(locationTable.Slice(glyphIndex * 4, 4)) :
                (uint)BinaryPrimitives.ReadUInt16BigEndian(locationTable.Slice(glyphIndex * 2, 2)) * 2;
        }
        return glyphOffsets;
    }
}
