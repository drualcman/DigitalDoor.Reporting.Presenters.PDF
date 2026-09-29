namespace DigitalDoor.Reporting.Presenters.PDF.Rendering.Fonts.Subsetting;

internal readonly struct TrueTypeTableRecord
{
    public int Offset { get; }
    public int Length { get; }

    public TrueTypeTableRecord(int offset, int length)
    {
        Offset = offset;
        Length = length;
    }
}
