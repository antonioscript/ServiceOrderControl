using MediatR;
using OsService.Application.V1.Features.Customers.GetCustomerById;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.Features.Customers.GetCustomerByContact;

public sealed record GetCustomerByContactQuery(
    string? Phone,
    string? Document)
    : IRequest<Result<GetCustomerByContactResponse>>;
