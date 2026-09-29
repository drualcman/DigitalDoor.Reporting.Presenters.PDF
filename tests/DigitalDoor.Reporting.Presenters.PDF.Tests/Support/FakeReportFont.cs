namespace DigitalDoor.Reporting.Presenters.PDF.Tests.Support;

public sealed class FakeReportFont : IReportFont
{
    private readonly Func<string, byte[]> ProvideFontBytes;
    private int RequestsCount;

    public int Requests => RequestsCount;

    public FakeReportFont(Func<string, byte[]> provideFontBytes)
    {
        ProvideFontBytes = provideFontBytes;
    }

    public byte[] GetFontBytesArray(string fontName)
    {
        Interlocked.Increment(ref RequestsCount);
        return ProvideFontBytes(fontName);
    }
}
