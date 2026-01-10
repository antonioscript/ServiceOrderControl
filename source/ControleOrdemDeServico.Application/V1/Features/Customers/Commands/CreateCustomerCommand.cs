using MediatR;

namespace OsService.Application.V1.Features.Customers.Commands;

public sealed record CreateCustomerCommand(
    string Name,
    string? Phone,
    string? Email,
    string? Document
) : IRequest<Guid>;
