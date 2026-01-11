using MediatR;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.Features.Customers.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid Id)
    : IRequest<Result<GetCustomerByContactResponse>>;
