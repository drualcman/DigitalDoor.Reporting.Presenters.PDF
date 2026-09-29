namespace DigitalDoor.Reporting.Presenters.PDF.Tests;

public class FontTests
{
    private const string CustomFontName = "MyCorporateFont";

    [Fact]
    public async Task CustomFontFromIReportFontIsEmbedded()
    {
        FakeReportFont reportFont = new FakeReportFont(fontName => TestFonts.ReadFontFile(TestFonts.BoldItalicFontFile));

        byte[] pdf = await GenerateWithFontAsync(CustomFontName, reportFont);

        Assert.Contains(PdfInspector.GetFontNames(pdf), fontName => fontName.Contains("LiberationSans-BoldItalic"));
        Assert.Equal(1, reportFont.Requests);
    }

    [Fact]
    public async Task StandardFontNamesDoNotCallIReportFont()
    {
        FakeReportFont reportFont = new FakeReportFont(fontName => throw new FileNotFoundException(fontName));

        byte[] pdf = await GenerateWithFontAsync("Helvetica", reportFont);

        Assert.Equal(0, reportFont.Requests);
        Assert.NotEmpty(PdfInspector.GetFontNames(pdf));
    }

    [Fact]
    public async Task MissingCustomFontUsesTheFallbackFont()
    {
        FakeReportFont reportFont = new FakeReportFont(fontName => null);

        byte[] pdf = await GenerateWithFontAsync(CustomFontName, reportFont);

        Assert.Contains("Texto con acentos áéíóú ñ €", string.Join(" ", PdfInspector.GetPageTexts(pdf)));
    }

    [Fact]
    public async Task IReportFontErrorsAreNotHidden()
    {
        FakeReportFont reportFont = new FakeReportFont(fontName => throw new FileNotFoundException(fontName));

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() => GenerateWithFontAsync(CustomFontName, reportFont));

        Assert.IsType<FileNotFoundException>(exception.InnerException);
    }

    [Fact]
    public async Task InvalidFontBytesThrowADescriptiveException()
    {
        FakeReportFont reportFont = new FakeReportFont(fontName => new byte[] { 1, 2, 3, 4 });

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() => GenerateWithFontAsync(CustomFontName, reportFont));

        Assert.Contains(CustomFontName, exception.Message);
    }

    [Fact]
    public async Task EmbeddedFontsAreSubset()
    {
        FakeReportFont reportFont = new FakeReportFont(fontName => TestFonts.ReadFontFile(TestFonts.RegularFontFile));

        byte[] pdf = await GenerateWithFontAsync(CustomFontName, reportFont);

        Assert.True(pdf.Length < TestFonts.ReadFontFile(TestFonts.RegularFontFile).Length / 2,
            $"The PDF has {pdf.Length} bytes, the font was not subset.");
    }

    private static async Task<byte[]> GenerateWithFontAsync(string fontName, IReportFont reportFont)
    {
        Setup setup = new Setup(PageSize.A4);
        setup.Body.Row = new Row(new Dimension(190, 20));
        setup.Body.AddColumn("Text", TestSetups.CreateTextFormat(0, 0, 190, 20, fontName, 12));
        List<ColumnData> data = new List<ColumnData> { new ColumnData(new Item("Text"), "Texto con acentos áéíóú ñ €", SectionType.Body) };
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(setup, data, reportFont);
        return report.Pdf;
    }
}
