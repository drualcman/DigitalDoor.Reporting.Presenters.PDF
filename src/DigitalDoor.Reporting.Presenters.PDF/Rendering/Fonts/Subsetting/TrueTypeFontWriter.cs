using System.Buffers.Binary;

namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts.Subsetting;

internal static class TrueTypeFontWriter
{
    private const uint TrueTypeOutlinesVersion = 0x00010000;
    private const uint ChecksumAdjustmentMagic = 0xB1B0AFBA;
    private const int HeadChecksumAdjustmentOffset = 8;
    private const int TableDirectoryHeaderSize = 12;
    private const int TableRecordSize = 16;

    public static byte[] Write(SortedDictionary<string, byte[]> tablesByTag)
    {
        int numberOfTables = tablesByTag.Count;
        int entrySelector = (int)Math.Floor(Math.Log(numberOfTables, 2));
        int searchRange = (1 << entrySelector) * TableRecordSize;
        int tableDataOffset = TableDirectoryHeaderSize + numberOfTables * TableRecordSize;
        int totalLength = tableDataOffset + tablesByTag.Values.Sum(table => AlignToFourBytes(table.Length));
        byte[] fontData = new byte[totalLength];
        Span<byte> fontSpan = fontData.AsSpan();
        BinaryPrimitives.WriteUInt32BigEndian(fontSpan.Slice(0, 4), TrueTypeOutlinesVersion);
        BinaryPrimitives.WriteUInt16BigEndian(fontSpan.Slice(4, 2), (ushort)numberOfTables);
        BinaryPrimitives.WriteUInt16BigEndian(fontSpan.Slice(6, 2), (ushort)searchRange);
        BinaryPrimitives.WriteUInt16BigEndian(fontSpan.Slice(8, 2), (ushort)entrySelector);
        BinaryPrimitives.WriteUInt16BigEndian(fontSpan.Slice(10, 2), (ushort)(numberOfTables * TableRecordSize - searchRange));
        int recordOffset = TableDirectoryHeaderSize;
        int currentTableOffset = tableDataOffset;
        int headTableOffset = -1;
        foreach (KeyValuePair<string, byte[]> table in tablesByTag)
        {
            Encoding.ASCII.GetBytes(table.Key, fontSpan.Slice(recordOffset, 4));
            table.Value.CopyTo(fontSpan.Slice(currentTableOffset, table.Value.Length));
            BinaryPrimitives.WriteUInt32BigEndian(fontSpan.Slice(recordOffset + 4, 4), CalculateChecksum(fontSpan.Slice(currentTableOffset, AlignToFourBytes(table.Value.Length))));
            BinaryPrimitives.WriteUInt32BigEndian(fontSpan.Slice(recordOffset + 8, 4), (uint)currentTableOffset);
            BinaryPrimitives.WriteUInt32BigEndian(fontSpan.Slice(recordOffset + 12, 4), (uint)table.Value.Length);
            headTableOffset = table.Key == "head" ? currentTableOffset : headTableOffset;
            recordOffset += TableRecordSize;
            currentTableOffset += AlignToFourBytes(table.Value.Length);
        }
        uint checksumAdjustment = unchecked(ChecksumAdjustmentMagic - CalculateChecksum(fontSpan));
        BinaryPrimitives.WriteUInt32BigEndian(fontSpan.Slice(headTableOffset + HeadChecksumAdjustmentOffset, 4), checksumAdjustment);
        return fontData;
    }

    private static uint CalculateChecksum(ReadOnlySpan<byte> alignedData)
    {
        uint checksum = 0;
        for (int position = 0; position + 4 <= alignedData.Length; position += 4)
        {
            checksum = unchecked(checksum + BinaryPrimitives.ReadUInt32BigEndian(alignedData.Slice(position, 4)));
        }
        return checksum;
    }

    private static int AlignToFourBytes(int length)
    {
        return (length + 3) & ~3;
    }
}
