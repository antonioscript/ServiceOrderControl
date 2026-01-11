using MediatR;

namespace OsService.Application.V1.UseCases.ServiceOrders.Command;

public sealed record OpenServiceOrderCommand(
    Guid CustomerId,
    string Description
) : IRequest<(Guid Id, int Number)>;
