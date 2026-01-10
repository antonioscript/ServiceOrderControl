using Microsoft.EntityFrameworkCore;
using OsService.Domain.Entities;

namespace OsService.Infrastructure.Databases;

public class OsServiceDbContext : DbContext
{
    public OsServiceDbContext(DbContextOptions<OsServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();
    public DbSet<ServiceOrderEntity> ServiceOrders => Set<ServiceOrderEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OsServiceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}