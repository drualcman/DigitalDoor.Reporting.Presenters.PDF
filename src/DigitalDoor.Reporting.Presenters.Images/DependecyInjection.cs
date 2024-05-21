using DigitalDoor.Reporting.Entities.Interfaces;
using DigitalDoor.Reporting.Presenters.Images;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesDependency
{
    public static IServiceCollection AddReportingPresenterImageServices(this IServiceCollection services)
    {
        services.AddReportingServices();
        services.TryAddScoped<IReportAsBytes<JPGPresenter>, JPGPresenter>();
        return services;
    }
}
