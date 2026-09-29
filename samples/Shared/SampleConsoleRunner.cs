using DigitalDoor.Reporting.Entities.Helpers;
using DigitalDoor.Reporting.Entities.Interfaces;
using DigitalDoor.Reporting.Entities.Models;
using DigitalDoor.Reporting.Entities.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalDoor.Reporting.Presenters.PDF.Samples;

public static class SampleConsoleRunner
{
    public const int PortraitBodyRows = 60;
    public const int LandscapeBodyRows = 50;

    public static async Task RunAsync(string outputDirectory, string filePrefix, bool includePlainBase64Strings)
    {
        Directory.CreateDirectory(outputDirectory);
        ServiceCollection services = new ServiceCollection();
        services.AddReportingPresenterPdfServices();
        using ServiceProvider provider = services.BuildServiceProvider();

        await GenerateVariantAsync(provider, outputDirectory, $"{filePrefix}-portrait",
            SampleReportSetupFactory.CreatePortraitSetup(), SampleReportDataFactory.CreateData(PortraitBodyRows, includePlainBase64Strings));
        await GenerateVariantAsync(provider, outputDirectory, $"{filePrefix}-landscape",
            SampleReportSetupFactory.CreateLandscapeSetup(), SampleReportDataFactory.CreateData(LandscapeBodyRows, includePlainBase64Strings));
    }

    private static async Task GenerateVariantAsync(ServiceProvider provider, string outputDirectory, string fileName,
        Setup setup, List<ColumnData> data)
    {
        using IServiceScope scope = provider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IReportsOutputPort>().Handle(setup, data);
        ReportViewModel reportViewModel = scope.ServiceProvider.GetRequiredService<IReportsPresenter>().Content;
        byte[] pdf = await scope.ServiceProvider.GetRequiredService<IReportAsBytes>().GenerateReport(reportViewModel);
        string pdfPath = Path.Combine(outputDirectory, $"{fileName}.pdf");
        await File.WriteAllBytesAsync(pdfPath, pdf);
        string html = new ReportHtmlGenerator(reportViewModel).GenerateHtml();
        await File.WriteAllTextAsync(Path.Combine(outputDirectory, $"{fileName}.html"), SampleHtmlDocument.Wrap(fileName, html, reportViewModel));
        Console.WriteLine($"{pdfPath}: {pdf.Length} bytes, {reportViewModel.Pages} pages according to the view model");
    }
}
