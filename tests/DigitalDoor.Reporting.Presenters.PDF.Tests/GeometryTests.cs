using UglyToad.PdfPig.Content;

namespace DigitalDoor.Reporting.Presenters.PDF.Tests;

public class GeometryTests
{
    private const double PointsPerMillimeter = 72d / 25.4d;
    private const double PositionTolerance = 1.5;
    private const double PageSizeTolerance = 0.5;

    [Fact]
    public async Task PortraitA4UsesIsoPointSize()
    {
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(TestSetups.CreatePaginatedSetup(), TestSetups.CreateData(1));

        (double width, double height) = PdfInspector.GetPageSize(report.Pdf, 1);

        Assert.Equal(595.28, width, PageSizeTolerance);
        Assert.Equal(841.89, height, PageSizeTolerance);
    }

    [Fact]
    public async Task LandscapeSwapsWidthAndHeight()
    {
        Setup setup = TestSetups.CreatePaginatedSetup();
        setup.Page.Orientation = Orientation.Landscape;

        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(setup, TestSetups.CreateData(1));
        (double width, double height) = PdfInspector.GetPageSize(report.Pdf, 1);

        Assert.Equal(841.89, width, PageSizeTolerance);
        Assert.Equal(595.28, height, PageSizeTolerance);
    }

    [Fact]
    public async Task PageMarginsMoveTheContent()
    {
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(TestSetups.CreatePaginatedSetup(), TestSetups.CreateData(1));

        Letter firstHeaderLetter = PdfInspector.GetLetters(report.Pdf, 1).First(letter => letter.Value == "H");

        Assert.InRange(firstHeaderLetter.StartBaseLine.X, 10 * PointsPerMillimeter - PositionTolerance, 10 * PointsPerMillimeter + PositionTolerance);
    }

    [Fact]
    public async Task SectionsAreStackedFromTheTopOfThePage()
    {
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(TestSetups.CreatePaginatedSetup(), TestSetups.CreateData(1));
        List<Letter> letters = PdfInspector.GetLetters(report.Pdf, 1);
        double pageHeight = PdfInspector.GetPageSize(report.Pdf, 1).Height;

        double headerTopFromPageTop = pageHeight - letters.First(letter => letter.Value == "H").BoundingBox.Top;
        double bodyTopFromPageTop = pageHeight - letters.First(letter => letter.Value == "w").BoundingBox.Top;

        Assert.InRange(headerTopFromPageTop, 10 * PointsPerMillimeter, 14 * PointsPerMillimeter);
        Assert.InRange(bodyTopFromPageTop, 30 * PointsPerMillimeter, 34 * PointsPerMillimeter);
    }

    [Fact]
    public async Task LongTextIsWrappedAndClippedInsideTheCell()
    {
        Setup setup = TestSetups.CreatePaginatedSetup(rowHeight: 20);
        List<ColumnData> data = TestSetups.CreateData(0);
        string longText = string.Join(" ", Enumerable.Range(1, 80).Select(wordNumber => $"word{wordNumber}"));
        data.Add(new ColumnData(new Item(TestSetups.BodyText), longText, SectionType.Body) { Row = 1 });

        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(setup, data);
        List<Letter> bodyLetters = PdfInspector.GetLetters(report.Pdf, 1).Where(letter => letter.Value == "w").ToList();
        int distinctLines = bodyLetters.Select(letter => Math.Round(letter.StartBaseLine.Y)).Distinct().Count();
        double maximumRight = bodyLetters.Max(letter => letter.BoundingBox.Right);

        Assert.InRange(distinctLines, 2, 5);
        Assert.True(maximumRight <= (10 + 90) * PointsPerMillimeter + PositionTolerance);
        Assert.DoesNotContain("word80", PdfInspector.GetPageTexts(report.Pdf)[0]);
    }
}
