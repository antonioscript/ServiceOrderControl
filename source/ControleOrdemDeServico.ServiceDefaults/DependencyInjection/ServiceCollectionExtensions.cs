using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OsService.ServiceDefaults.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddModules(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        var moduleType = typeof(IModule);

        var modules = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && moduleType.IsAssignableFrom(t))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();

        foreach (var module in modules)
        {
            module.ConfigureServices(services, configuration);
        }
    }
}