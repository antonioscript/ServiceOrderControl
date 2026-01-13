using MediatR;
using Microsoft.AspNetCore.Http;
using OsService.Domain.Enums;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.UploadServiceOrderAttachment;

public partial class UploadServiceOrderAttachment
{
    public sealed record Command(
        Guid ServiceOrderId,
        AttachmentType Type,
        IFormFile File
    ) : IRequest<Result<Response>>;
}
