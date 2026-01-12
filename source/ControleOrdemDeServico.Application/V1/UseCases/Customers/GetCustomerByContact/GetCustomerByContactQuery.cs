using MediatR;
using OsService.Application.V1.UseCases.Customers.GetCustomerById;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers.GetCustomerByContact;

public sealed record GetCustomerByContactQuery(
    string? Phone,
    string? Document)
    : IRequest<Result<GetCustomerByContactResponse>>;
