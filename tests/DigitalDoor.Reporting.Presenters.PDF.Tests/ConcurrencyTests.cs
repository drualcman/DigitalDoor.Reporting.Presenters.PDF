using System.Collections.Concurrent;

namespace DigitalDoor.Reporting.Presenters.PDF.Tests;

public class ConcurrencyTests
{
    private const int ConcurrentReports = 40;
    private const string CustomFontName = "MyCorporateFont";

    [Fact]
    public async Task ConcurrentReportsUseTheirOwnFontAndPagination()
    {
        byte[] regularFont = TestFonts.ReadFontFile(TestFonts.RegularFontFile);
        byte[] boldItalicFont = TestFonts.ReadFontFile(TestFonts.BoldItalicFontFile);
        ConcurrentBag<string> failures = new ConcurrentBag<string>();

        await Parallel.ForEachAsync(Enumerable.Range(0, ConcurrentReports), new ParallelOptions { MaxDegreeOfParallelism = 8 },
            async (reportIndex, cancellationToken) =>
            {
                bool useBoldItalic = reportIndex % 2 == 0;
                int bodyRows = 5 + reportIndex;
                FakeReportFont reportFont = new FakeReportFont(fontName => useBoldItalic ? boldItalicFont : regularFont);
                Setup setup = CreateCustomFontSetup();
                GeneratedTestReport report = await TestReportGenerator.GenerateAsync(setup, TestSetups.CreateData(bodyRows), reportFont);
                string expectedFont = useBoldItalic ? "LiberationSans-BoldItalic" : "LiberationSans";
                bool usesExpectedFont = PdfInspector.GetFontNames(report.Pdf).Any(fontName => fontName.EndsWith(expectedFont));
                int expectedPages = (int)Math.Ceiling(bodyRows / 10d);
                if (!usesExpectedFont || PdfInspector.GetPageCount(report.Pdf) != expectedPages)
                {
                    failures.Add($"Report {reportIndex} has unexpected font or pages.");
                }
            });

        Assert.Empty(failures);
    }

    [Fact]
    public async Task SamePresenterInstanceReturnsTheResultOfEachCall()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddReportingPresenterPdfServices();
        using ServiceProvider provider = services.BuildServiceProvider();
        using IServiceScope scope = provider.CreateScope();
        IReportAsBytes reportAsBytes = scope.ServiceProvider.GetRequiredService<IReportAsBytes>();
        ReportViewModel onePageReport = new ReportViewModel(TestSetups.CreatePaginatedSetup(), TestSetups.CreateData(5));
        ReportViewModel fourPagesReport = new ReportViewModel(TestSetups.CreatePaginatedSetup(), TestSetups.CreateData(35));

        Task<byte[]>[] generations = Enumerable.Range(0, 20)
            .Select(index => Task.Run(() => reportAsBytes.GenerateReport(index % 2 == 0 ? onePageReport : fourPagesReport)))
            .ToArray();
        byte[][] pdfs = await Task.WhenAll(generations);

        for (int index = 0; index < pdfs.Length; index++)
        {
            Assert.Equal(index % 2 == 0 ? 1 : 4, PdfInspector.GetPageCount(pdfs[index]));
        }
    }

    private static Setup CreateCustomFontSetup()
    {
        Setup setup = TestSetups.CreatePaginatedSetup();
        setup.Body.Items.Clear();
        setup.Body.AddColumn(TestSetups.BodyText, TestSetups.CreateTextFormat(0, 0, 90, 10, CustomFontName));
        return setup;
    }
}
