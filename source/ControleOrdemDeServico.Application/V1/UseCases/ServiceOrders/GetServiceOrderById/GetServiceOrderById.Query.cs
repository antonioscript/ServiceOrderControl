using MediatR;
using OsService.Domain.ResultPattern;
using static OsService.Application.V1.UseCases.ServiceOrders.OpenServiceOrder.OpenServiceOrder;

namespace OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderById;

public partial class GetServiceOrderById
{
    public sealed record Query(Guid Id) : IRequest<Result<Response>>;
}