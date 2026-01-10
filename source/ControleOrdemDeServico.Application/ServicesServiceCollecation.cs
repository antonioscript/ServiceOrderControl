using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OsService.Application;   
public static class ServicesServiceCollection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ServicesServiceCollection).Assembly));

        return services;
    }
}
