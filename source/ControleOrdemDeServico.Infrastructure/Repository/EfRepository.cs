using Microsoft.EntityFrameworkCore;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Infrastructure.Databases;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OsService.Infrastructure.Repository;

//TODO: Colocar no Readme a explicação das Classes Virtuais

public class EfRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    protected readonly OsServiceDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    public EfRepository(OsServiceDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = DbContext.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await DbSet.FindAsync(new object[] { id }, ct);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct)
    {
        await DbSet.AddAsync(entity, ct);
    }

    public virtual Task UpdateAsync(TEntity entity, CancellationToken ct)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task RemoveAsync(TEntity entity, CancellationToken ct)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
    {
        var entity = await DbSet.FindAsync(new object[] { id }, ct);
        return entity != null;
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken ct)
    {
        return await DbSet.AsNoTracking().ToListAsync(ct);
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct)
    {
        return await DbSet.AsNoTracking()
            .Where(predicate)
            .ToListAsync(ct);
    }
}