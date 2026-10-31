namespace DigitalDoor.Reporting.Presenters.PDF.Tests;

public class NullSafetyTests
{
    public static TheoryData<string> MissingParts => new TheoryData<string>
    {
        "data", "page", "header", "body", "footer", "formats", "rows", "items"
    };

    [Fact]
    public void HtmlOfANullReportIsEmpty()
    {
        string html = new ReportHtmlGenerator(null).GenerateHtml();

        Assert.Equal(string.Empty, html);
    }

    [Theory]
    [MemberData(nameof(MissingParts))]
    public void HtmlIsGeneratedWhenAPartOfTheReportIsNull(string missingPart)
    {
        ReportViewModel report = CreateReportWithout(missingPart);

        string html = new ReportHtmlGenerator(report).GenerateHtml();

        Assert.Contains("dd-report-page", html);
    }

    [Theory]
    [MemberData(nameof(MissingParts))]
    public async Task PdfIsGeneratedWhenAPartOfTheReportIsNull(string missingPart)
    {
        ReportViewModel report = CreateReportWithout(missingPart);

        byte[] pdf = await TestReportGenerator.GenerateFromViewModelAsync(report);

        Assert.True(PdfInspector.GetPageCount(pdf) >= 1);
    }

    private static ReportViewModel CreateReportWithout(string missingPart)
    {
        ReportViewModel report = new ReportViewModel(TestSetups.CreatePaginatedSetup(), TestSetups.CreateData(5));
        if (missingPart == "data")
        {
            report.Data = null;
        }
        else if (missingPart == "page")
        {
            report.Page = null;
        }
        else if (missingPart == "header")
        {
            report.Header = null;
        }
        else if (missingPart == "body")
        {
            report.Body = null;
        }
        else if (missingPart == "footer")
        {
            report.Footer = null;
        }
        else if (missingPart == "formats")
        {
            report.Header.Format = null;
            report.Body.Format = null;
            report.Footer.Format = null;
        }
        else if (missingPart == "rows")
        {
            report.Header.Row = null;
            report.Body.Row = null;
            report.Footer.Row = null;
        }
        else
        {
            report.Header.Items = null;
            report.Body.Items = null;
            report.Footer.Items = null;
        }
        return report;
    }
}
