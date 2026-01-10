using OsService.Domain.Entities;

namespace OsService.Application.V1.Abstractions.Persistence;

public interface ICustomerRepository : IRepository<CustomerEntity>
{
}