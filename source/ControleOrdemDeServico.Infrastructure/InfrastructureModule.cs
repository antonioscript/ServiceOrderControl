using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Infrastructure.Databases;
using OsService.Infrastructure.Repository;
using OsService.ServiceDefaults.DependencyInjection;

namespace OsService.Infrastructure;

public sealed class InfrastructureModule : IModule
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection string not configured");

        services.AddDbContext<OsServiceDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        services.AddScoped<IServiceOrderRepository, ServiceOrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<DatabaseGenerantor>();
    }
}