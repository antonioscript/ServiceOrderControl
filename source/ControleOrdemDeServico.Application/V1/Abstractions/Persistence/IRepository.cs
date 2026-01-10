using System.Linq.Expressions;

namespace OsService.Application.V1.Abstractions.Persistence;

//TODO: Verificar se esse é o melhor local para colocar essa interface
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct);

    Task AddAsync(TEntity entity, CancellationToken ct);

    Task UpdateAsync(TEntity entity, CancellationToken ct);

    Task RemoveAsync(TEntity entity, CancellationToken ct);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken ct);
    Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct);
}