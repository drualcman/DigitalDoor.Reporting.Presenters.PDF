namespace DigitalDoor.Reporting.Presenters.PDF;

public static class ServicesDependency
{
    public static IServiceCollection AddReportingServices(this IServiceCollection services)
    {
        services.TryAddScoped<PDFReportPresenter>();
        services.TryAddScoped<IPDFReportOutputPort>(service => service.GetService<PDFReportPresenter>());
        services.TryAddScoped<IPDFReportPresenter>(services => services.GetService<PDFReportPresenter>());

        services.TryAddScoped<IReportAsBytes, PDFReportController>();

        return services;
    }
}
