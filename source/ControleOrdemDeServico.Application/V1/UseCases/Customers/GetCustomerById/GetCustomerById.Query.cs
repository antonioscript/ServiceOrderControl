using MediatR;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers.GetCustomerById;

public partial class GetCustomerById 
{
    public sealed record Query(Guid Id)
    : IRequest<Result<Response>>;
}



