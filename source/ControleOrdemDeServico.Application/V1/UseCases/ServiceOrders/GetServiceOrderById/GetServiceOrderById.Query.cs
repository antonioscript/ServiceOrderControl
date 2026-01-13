using MediatR;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderById;

public partial class GetServiceOrderById
{
    public sealed record Query(Guid Id) : IRequest<Result<Response>>;
}