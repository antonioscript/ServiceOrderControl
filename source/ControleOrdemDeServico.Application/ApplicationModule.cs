using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OsService.ServiceDefaults.DependencyInjection;

namespace OsService.Application;

public sealed class ApplicationModule : IModule
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationModule).Assembly));

        services.AddAutoMapper(
            cfg => { },                 
            typeof(ApplicationModule).Assembly
        );
    }
}