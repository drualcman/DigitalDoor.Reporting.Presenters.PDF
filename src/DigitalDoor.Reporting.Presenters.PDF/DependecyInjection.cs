﻿namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesDependency
{
    public static IServiceCollection AddReportingPresenterPdfServices(this IServiceCollection services)
    {
        services.AddReportingServices();

        services.TryAddScoped<IReportAsBytes<PDFReportPresenter>, PDFReportPresenter>();
        services.TryAddScoped<IReportAsBytes, PDFReportController>();
        return services;
    }
}
