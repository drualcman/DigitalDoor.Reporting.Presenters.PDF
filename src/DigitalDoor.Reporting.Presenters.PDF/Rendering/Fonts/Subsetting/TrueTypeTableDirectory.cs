using System.Buffers.Binary;

namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts.Subsetting;

internal sealed class TrueTypeTableDirectory
{
    private const uint TrueTypeOutlinesVersion = 0x00010000;
    private const uint AppleTrueTypeOutlinesVersion = 0x74727565;
    private const int TableDirectoryHeaderSize = 12;
    private const int TableRecordSize = 16;
    private static readonly string[] RequiredTables = { "head", "maxp", "loca", "glyf", "hhea", "hmtx", "cmap" };

    private readonly byte[] FontData;
    private readonly Dictionary<string, TrueTypeTableRecord> Tables;

    public bool HasTrueTypeOutlines { get; }

    private TrueTypeTableDirectory(byte[] fontData, Dictionary<string, TrueTypeTableRecord> tables, bool hasTrueTypeOutlines)
    {
        FontData = fontData;
        Tables = tables;
        HasTrueTypeOutlines = hasTrueTypeOutlines;
    }

    public static TrueTypeTableDirectory Parse(byte[] fontData)
    {
        uint sfntVersion = BinaryPrimitives.ReadUInt32BigEndian(fontData.AsSpan(0, 4));
        int numberOfTables = BinaryPrimitives.ReadUInt16BigEndian(fontData.AsSpan(4, 2));
        Dictionary<string, TrueTypeTableRecord> tables = new Dictionary<string, TrueTypeTableRecord>(StringComparer.Ordinal);
        for (int tableIndex = 0; tableIndex < numberOfTables; tableIndex++)
        {
            int recordOffset = TableDirectoryHeaderSize + tableIndex * TableRecordSize;
            string tag = Encoding.ASCII.GetString(fontData, recordOffset, 4);
            int tableOffset = checked((int)BinaryPrimitives.ReadUInt32BigEndian(fontData.AsSpan(recordOffset + 8, 4)));
            int tableLength = checked((int)BinaryPrimitives.ReadUInt32BigEndian(fontData.AsSpan(recordOffset + 12, 4)));
            if (tableOffset >= 0 && tableLength >= 0 && (long)tableOffset + tableLength <= fontData.Length)
            {
                tables[tag] = new TrueTypeTableRecord(tableOffset, tableLength);
            }
        }
        bool hasTrueTypeOutlines = (sfntVersion == TrueTypeOutlinesVersion || sfntVersion == AppleTrueTypeOutlinesVersion) &&
            RequiredTables.All(tables.ContainsKey);
        return new TrueTypeTableDirectory(fontData, tables, hasTrueTypeOutlines);
    }

    public bool HasTable(string tag)
    {
        return Tables.ContainsKey(tag);
    }

    public ReadOnlySpan<byte> GetTable(string tag)
    {
        TrueTypeTableRecord record = Tables[tag];
        return new ReadOnlySpan<byte>(FontData, record.Offset, record.Length);
    }
}
