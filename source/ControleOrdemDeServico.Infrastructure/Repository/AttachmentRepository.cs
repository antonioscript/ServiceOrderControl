using Microsoft.EntityFrameworkCore;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Infrastructure.Databases;

namespace OsService.Infrastructure.Repository;

public sealed class AttachmentRepository : EfRepository<AttachmentEntity>, IAttachmentRepository
{
    private readonly OsServiceDbContext _dbContext;

    public AttachmentRepository(OsServiceDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<AttachmentEntity>> ListByServiceOrderIdAsync(Guid serviceOrderId,CancellationToken ct)
    {
        return await _dbContext.Attachments
            .Where(a => a.ServiceOrderId == serviceOrderId)
            .OrderBy(a => a.UploadedAt)
            .ToListAsync(ct);
    }
}
