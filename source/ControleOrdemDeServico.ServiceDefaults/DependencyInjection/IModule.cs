using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OsService.ServiceDefaults.DependencyInjection;


public interface IModule
{
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}