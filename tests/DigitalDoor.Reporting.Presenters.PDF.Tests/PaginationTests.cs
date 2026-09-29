namespace DigitalDoor.Reporting.Presenters.PDF.Tests;

public class PaginationTests
{
    [Fact]
    public async Task HeaderAndFooterShowCurrentAndTotalPagesOnEveryPage()
    {
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(TestSetups.CreatePaginatedSetup(), TestSetups.CreateData(25));

        List<string> pageTexts = PdfInspector.GetPageTexts(report.Pdf);

        Assert.Equal(3, pageTexts.Count);
        for (int pageIndex = 0; pageIndex < pageTexts.Count; pageIndex++)
        {
            string expectedPagination = $"{pageIndex + 1} 3";
            Assert.StartsWith($"HEADER {expectedPagination}", pageTexts[pageIndex]);
            Assert.EndsWith($"FOOTER {expectedPagination}", pageTexts[pageIndex]);
        }
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 1)]
    [InlineData(11, 2)]
    [InlineData(20, 2)]
    [InlineData(21, 3)]
    public async Task TotalPagesMatchesTheBodyRows(int bodyRows, int expectedPages)
    {
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(TestSetups.CreatePaginatedSetup(), TestSetups.CreateData(bodyRows));

        Assert.Equal(expectedPages, PdfInspector.GetPageCount(report.Pdf));
        Assert.Equal(expectedPages, report.ViewModel.Pages);
        Assert.Contains($"1 {expectedPages}", PdfInspector.GetPageTexts(report.Pdf)[0]);
    }

    [Fact]
    public async Task BodyColumnsAreFilledBeforeCreatingANewPage()
    {
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(
            TestSetups.CreatePaginatedSetup(columnsNumber: 2), TestSetups.CreateData(25));

        List<string> pageTexts = PdfInspector.GetPageTexts(report.Pdf);

        Assert.Equal(2, pageTexts.Count);
        Assert.Contains("Row20", pageTexts[0]);
        Assert.Contains("Row21", pageTexts[1]);
    }

    [Fact]
    public async Task EmptyBodyGeneratesOnePage()
    {
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(TestSetups.CreatePaginatedSetup(), TestSetups.CreateData(0));

        Assert.Equal(1, PdfInspector.GetPageCount(report.Pdf));
        Assert.Contains("HEADER 1 1", PdfInspector.GetPageTexts(report.Pdf)[0]);
    }

    [Fact]
    public async Task RowHigherThanBodyDoesNotLoopForever()
    {
        Setup setup = TestSetups.CreatePaginatedSetup(bodyHeight: 5, rowHeight: 10);

        Task<GeneratedTestReport> generation = TestReportGenerator.GenerateAsync(setup, TestSetups.CreateData(3));
        Task finishedTask = await Task.WhenAny(generation, Task.Delay(TimeSpan.FromSeconds(30)));

        Assert.Same(generation, finishedTask);
        Assert.Equal(3, PdfInspector.GetPageCount(generation.Result.Pdf));
    }

    [Fact]
    public async Task PaginationWorksWhenTheViewModelIsBuiltWithoutReportsPresenter()
    {
        Setup setup = TestSetups.CreatePaginatedSetup();
        ReportViewModel viewModel = new ReportViewModel(setup, TestSetups.CreateData(15));

        byte[] pdf = await TestReportGenerator.GenerateFromViewModelAsync(viewModel);

        Assert.Contains("HEADER 2 2", PdfInspector.GetPageTexts(pdf)[1]);
    }
}
