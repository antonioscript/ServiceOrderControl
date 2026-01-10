using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Infrastructure.Databases;

namespace OsService.Infrastructure.Repository;

public sealed class CustomerRepository
    : EfRepository<CustomerEntity>, ICustomerRepository
{
    public CustomerRepository(OsServiceDbContext dbContext)
        : base(dbContext)
    {
    }
}