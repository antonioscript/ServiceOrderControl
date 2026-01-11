using Microsoft.EntityFrameworkCore;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Infrastructure.Databases;

namespace OsService.Infrastructure.Repository;

public sealed class CustomerRepository
    : EfRepository<CustomerEntity>, ICustomerRepository
{
    private readonly OsServiceDbContext _dbContext;
    public CustomerRepository(OsServiceDbContext dbContext): base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsByDocumentAsync(string document, CancellationToken ct)
    {
        return await _dbContext.Customers
            .AnyAsync(c => c.Document == document, ct);
    }

    public async Task<bool> ExistsByPhoneAsync(string phone, CancellationToken ct)
    {
        return await _dbContext.Customers
            .AnyAsync(c => c.Phone == phone, ct);
    }
}