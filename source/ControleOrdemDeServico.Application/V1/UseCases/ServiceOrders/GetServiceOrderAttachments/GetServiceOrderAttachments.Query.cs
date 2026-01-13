using MediatR;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderAttachments;

public partial class GetServiceOrderAttachments
{
    public sealed record Query(Guid ServiceOrderId)
        : IRequest<Result<IReadOnlyList<Response>>>;
}
