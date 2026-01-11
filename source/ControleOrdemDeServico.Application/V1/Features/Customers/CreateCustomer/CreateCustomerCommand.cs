using MediatR;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.Features.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name,
    string? Phone,
    string? Email,
    string? Document
) : IRequest<Result<Guid>>;
