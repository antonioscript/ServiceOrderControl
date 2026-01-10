using OsService.Infrastructure.Databases;
using OsService.Application.V1.Abstractions.Persistence;

namespace OsService.Infrastructure.Repository;

//TODO: Verificar se esse é o melhor local para o Unit of Work
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly OsServiceDbContext _dbContext;

    public UnitOfWork(OsServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> CommitAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);
}