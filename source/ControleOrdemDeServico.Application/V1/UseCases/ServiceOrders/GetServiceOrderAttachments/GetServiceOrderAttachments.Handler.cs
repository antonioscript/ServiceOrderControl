using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderAttachments;

public partial class GetServiceOrderAttachments
{
    public sealed class Handler(
        IServiceOrderRepository serviceOrders,
        IAttachmentRepository attachments,
        IMapper mapper)
        : IRequestHandler<Query, Result<IReadOnlyList<Response>>>
    {
        public async Task<Result<IReadOnlyList<Response>>> Handle(
            Query request,
            CancellationToken ct)
        {
            var exists = await serviceOrders.ExistsAsync(request.ServiceOrderId, ct);
            if (!exists)
                return Result.Failure<IReadOnlyList<Response>>(ServiceOrderErrors.NotFound);

            var list = await attachments.ListByServiceOrderIdAsync(request.ServiceOrderId, ct);
            var response = mapper.Map<IReadOnlyList<Response>>(list);
            return Result.Success(response);
        }
    }
}
