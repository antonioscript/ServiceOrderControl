using OsService.Domain.Enums;

namespace OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderAttachments;

public partial class GetServiceOrderAttachments
{
    public sealed record Response(
        Guid Id,
        AttachmentType Type,
        string FileName,
        string ContentType,
        long SizeBytes,
        string StoragePath,
        DateTime UploadedAt
    );
}
