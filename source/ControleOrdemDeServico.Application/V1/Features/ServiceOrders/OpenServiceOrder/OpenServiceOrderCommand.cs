using MediatR;

namespace OsService.Application.V1.Features.ServiceOrders.Command;

public sealed record OpenServiceOrderCommand(
    Guid CustomerId,
    string Description
) : IRequest<(Guid Id, int Number)>;
