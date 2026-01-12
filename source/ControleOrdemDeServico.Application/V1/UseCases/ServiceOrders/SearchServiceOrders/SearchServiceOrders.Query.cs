using MediatR;
using OsService.Domain.Enums;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.SearchServiceOrders;

public partial class SearchServiceOrders
{
    public sealed record Query : IRequest<Result<IReadOnlyList<Response>>>
    {
        public Guid? CustomerId { get; init; }
        public ServiceOrderStatus? Status { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
    }

}