namespace OsService.Application.V1.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
