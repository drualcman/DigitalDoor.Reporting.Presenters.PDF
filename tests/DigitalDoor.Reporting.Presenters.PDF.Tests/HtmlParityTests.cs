using System.Globalization;
using System.Text.RegularExpressions;

namespace DigitalDoor.Reporting.Presenters.PDF.Tests;

public class HtmlParityTests
{
    private const string PageMarker = "class=\"dd-report-page\"";

    [Fact]
    public async Task HtmlAndPdfHaveTheSamePages()
    {
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(TestSetups.CreatePaginatedSetup(columnsNumber: 2), TestSetups.CreateData(45));

        string html = new ReportHtmlGenerator(report.ViewModel).GenerateHtml();

        Assert.Equal(PdfInspector.GetPageCount(report.Pdf), Regex.Matches(html, PageMarker).Count);
    }

    [Fact]
    public async Task HtmlRowsKeepTheirOrder()
    {
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(TestSetups.CreatePaginatedSetup(columnsNumber: 2), TestSetups.CreateData(45));

        string html = new ReportHtmlGenerator(report.ViewModel).GenerateHtml();
        List<int> rowPositions = Enumerable.Range(1, 45).Select(rowNumber => html.IndexOf($">Row{rowNumber}<", StringComparison.Ordinal)).ToList();

        Assert.DoesNotContain(-1, rowPositions);
        Assert.Equal(rowPositions.OrderBy(position => position), rowPositions);
    }

    [Fact]
    public async Task HtmlColumnsUseRowWidthAndColumnsSpaceLikeThePdf()
    {
        Setup setup = TestSetups.CreatePaginatedSetup(columnsNumber: 2);
        setup.Body.ColumnsSpace = 5;
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(setup, TestSetups.CreateData(15));

        string html = new ReportHtmlGenerator(report.ViewModel).GenerateHtml();

        Assert.Contains("left:105mm;top:30mm;", html);
    }

    [Fact]
    public async Task HtmlUsesInvariantNumbersAndEncodesText()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("es-ES");
        try
        {
            Setup setup = TestSetups.CreatePaginatedSetup(rowHeight: 10.5);
            List<ColumnData> data = TestSetups.CreateData(1, headerText: "<b>A & B</b>");
            GeneratedTestReport report = await TestReportGenerator.GenerateAsync(setup, data);

            string html = new ReportHtmlGenerator(report.ViewModel).GenerateHtml();

            Assert.Contains("height:10.5mm", html);
            Assert.DoesNotMatch(@"\d,\d+mm", html);
            Assert.Contains("&lt;b&gt;A &amp; B&lt;/b&gt;", html);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
