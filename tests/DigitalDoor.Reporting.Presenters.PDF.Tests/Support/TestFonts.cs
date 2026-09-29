namespace DigitalDoor.Reporting.Presenters.PDF.Tests.Support;

public static class TestFonts
{
    public const string RegularFontFile = "LiberationSans-Regular.ttf";
    public const string BoldItalicFontFile = "LiberationSans-BoldItalic.ttf";

    public static byte[] ReadFontFile(string fontFileName)
    {
        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fonts", fontFileName));
    }
}
