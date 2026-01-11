using OsService.Domain.Entities;

namespace OsService.Application.V1.Abstractions.Persistence;

public interface ICustomerRepository : IRepository<CustomerEntity>
{
    Task<bool> ExistsByDocumentAsync(string document, CancellationToken ct);
    Task<bool> ExistsByPhoneAsync(string phone, CancellationToken ct);
}