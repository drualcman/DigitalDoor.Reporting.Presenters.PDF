namespace DigitalDoor.Reporting.Presenters.PDF.Tests.Support;

public static class TestReportGenerator
{
    public static async Task<GeneratedTestReport> GenerateAsync(Setup setup, List<ColumnData> data, IReportFont reportFont = null)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddReportingPresenterPdfServices();
        if (reportFont is not null)
        {
            services.AddSingleton(reportFont);
        }
        using ServiceProvider provider = services.BuildServiceProvider();
        using IServiceScope scope = provider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IReportsOutputPort>().Handle(setup, data);
        ReportViewModel viewModel = scope.ServiceProvider.GetRequiredService<IReportsPresenter>().Content;
        byte[] pdf = await scope.ServiceProvider.GetRequiredService<IReportAsBytes>().GenerateReport(viewModel);
        return new GeneratedTestReport(pdf, viewModel);
    }

    public static async Task<byte[]> GenerateFromViewModelAsync(ReportViewModel viewModel, IReportFont reportFont = null)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddReportingPresenterPdfServices();
        if (reportFont is not null)
        {
            services.AddSingleton(reportFont);
        }
        using ServiceProvider provider = services.BuildServiceProvider();
        using IServiceScope scope = provider.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IReportAsBytes>().GenerateReport(viewModel);
    }
}
