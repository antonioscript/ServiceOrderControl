using OsService.Domain.Enums;

namespace OsService.Application.V1.UseCases.ServiceOrders.UploadServiceOrderAttachment;


public partial class UploadServiceOrderAttachment
{
    public sealed record Response(
        Guid Id,
        Guid ServiceOrderId,
        AttachmentType Type,
        string FileName,
        string ContentType,
        long SizeBytes,
        string StoragePath,
        DateTime UploadedAt
    );
}

