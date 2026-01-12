using OsService.Domain.Entities;

namespace OsService.Application.V1.Abstractions.Persistence;

public interface IAttachmentRepository : IRepository<AttachmentEntity>
{
    Task<IReadOnlyList<AttachmentEntity>> ListByServiceOrderIdAsync(
        Guid serviceOrderId,
        CancellationToken ct);
}
