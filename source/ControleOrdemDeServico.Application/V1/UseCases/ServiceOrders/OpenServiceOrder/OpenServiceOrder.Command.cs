using MediatR;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.OpenServiceOrder;

public partial class OpenServiceOrder
{
    public sealed record Command(
        Guid CustomerId,
        string Description,
        decimal? Price
    ) : IRequest<Result<Response>>;
}