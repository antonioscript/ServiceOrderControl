using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Infrastructure.Databases;

namespace OsService.Infrastructure.Repository;

public sealed class ServiceOrderRepository: EfRepository<ServiceOrderEntity>, IServiceOrderRepository
{
    public ServiceOrderRepository(OsServiceDbContext dbContext)
        : base(dbContext)
    {
    }
}