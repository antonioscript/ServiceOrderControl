using MediatR;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers.GetCustomerByContact;

public partial class GetCustomerByContact 
{
    public sealed record Query(
    string? Phone,
    string? Document)
    : IRequest<Result<Response>>;
}
