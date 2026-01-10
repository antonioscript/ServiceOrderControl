namespace OsService.Infrastructure.Databases;

public class DatabaseGenerantor
{
    private readonly OsServiceDbContext _dbContext;

    public DatabaseGenerantor(OsServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task EnsureCreatedAsync(CancellationToken ct)
    {

        return _dbContext.Database.EnsureCreatedAsync(ct);
    }
}
