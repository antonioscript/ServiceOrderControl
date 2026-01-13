namespace OsService.Domain.Entities;


using OsService.Domain.Common;
using OsService.Domain.Enums;

public sealed class AttachmentEntity : BaseEntity
{
    public Guid ServiceOrderId { get; init; }
    public AttachmentType Type { get; init; }

    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public long SizeBytes { get; init; }

    /// <summary>
    /// Caminho físico do arquivo (ex.: /data/uploads/{osId}/before/xxx.jpg)
    /// </summary>
    public string StoragePath { get; init; } = default!;

    public DateTime UploadedAt { get; init; }
}
