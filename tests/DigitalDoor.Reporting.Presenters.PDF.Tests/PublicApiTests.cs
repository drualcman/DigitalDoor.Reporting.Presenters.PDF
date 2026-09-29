namespace DigitalDoor.Reporting.Presenters.PDF.Tests;

public class PublicApiTests
{
    [Fact]
    public void ServicesAreRegisteredWithTheSameContracts()
    {
        ServiceCollection services = new ServiceCollection();

        services.AddReportingPresenterPdfServices();
        using ServiceProvider provider = services.BuildServiceProvider();
        using IServiceScope scope = provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetService<IReportAsBytes>());
        Assert.NotNull(scope.ServiceProvider.GetService<IReportsOutputPort>());
        Assert.NotNull(scope.ServiceProvider.GetService<IReportsPresenter>());
        Assert.Contains(services, descriptor => descriptor.ServiceType.IsGenericType &&
            descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IReportAsBytes<>));
    }

    [Fact]
    public async Task GeneratedBytesAreAPdfDocument()
    {
        GeneratedTestReport report = await TestReportGenerator.GenerateAsync(TestSetups.CreatePaginatedSetup(), TestSetups.CreateData(3));

        Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(report.Pdf, 0, 4));
    }

    [Fact]
    public void PresenterImplementsTheLegacyOutputPortInterfaces()
    {
        Type presenterType = typeof(DigitalDoor.Reporting.Presenters.PDF.Common.MeasurementConverter).Assembly.GetTypes().Single(type => type.Name == "PDFReportPresenter");

        Assert.True(typeof(IPDFReportPresenter).IsAssignableFrom(presenterType));
        Assert.True(typeof(IPDFReportOutputPort).IsAssignableFrom(presenterType));
    }
}
