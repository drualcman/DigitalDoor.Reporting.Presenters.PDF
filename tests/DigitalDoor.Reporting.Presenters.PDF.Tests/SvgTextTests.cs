namespace DigitalDoor.Reporting.Presenters.PDF.Tests;

public class SvgTextTests
{
    private const string ChartColumn = "Chart";
    private const int MaximumPdfSizeWithSubsetFonts = 150_000;

    [Fact]
    public async Task SvgTextIsDrawnWithTheReportFontsAndSubset()
    {
        string svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"200\" height=\"60\" font-family=\"Arial, Helvetica, sans-serif\">" +
            "<rect width=\"200\" height=\"60\" fill=\"#eeeeee\" /><text x=\"10\" y=\"25\" font-size=\"14\">Quarterly sales</text>" +
            "<text x=\"10\" y=\"50\" font-size=\"14\" font-weight=\"bold\">Total 1204</text></svg>";

        byte[] pdf = await GenerateWithSvgAsync(svg);
        string pageText = PdfInspector.GetPageTexts(pdf)[0];

        Assert.Contains("Quarterly", pageText);
        Assert.Contains("1204", pageText);
        Assert.True(pdf.Length < MaximumPdfSizeWithSubsetFonts, $"The PDF has {pdf.Length} bytes, so the SVG fonts were not subset.");
    }

    [Fact]
    public async Task SvgTextWithGenericFamilyAndStyleAttributeIsDrawn()
    {
        string svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"200\" height=\"40\">" +
            "<text x=\"10\" y=\"25\" style=\"font-family: 'serif'; font-style: italic; font-size: 14px\">Serif caption</text></svg>";

        byte[] pdf = await GenerateWithSvgAsync(svg);

        Assert.Contains("Serif", PdfInspector.GetPageTexts(pdf)[0]);
    }

    private static async Task<byte[]> GenerateWithSvgAsync(string svg)
    {
        Setup setup = new Setup(PageSize.A4);
        setup.Body.Row = new Row(new Dimension(190, 60));
        setup.Body.AddColumn(ChartColumn, new Format(100, 30) { Position = new Kernel(5, 5) });
        List<ColumnData> data = new List<ColumnData> { new ColumnData(new Item(ChartColumn), svg, SectionType.Body) };
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(setup, data);
        return report.Pdf;
    }
}
