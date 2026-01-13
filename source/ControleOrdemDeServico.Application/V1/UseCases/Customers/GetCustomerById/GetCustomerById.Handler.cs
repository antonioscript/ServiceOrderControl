using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers.GetCustomerById;


public partial class GetCustomerById 
{
    public sealed class GetCustomerByIdHandler(
    ICustomerRepository repo,
    IMapper mapper)
    : IRequestHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var entity = await repo.GetByIdAsync(request.Id, cancellationToken);

            if (entity is null)
                return Result.Failure<Response>(CustomerErrors.NotFound);

            var response = mapper.Map<Response>(entity);
            return Result.Success(response);
        }
    }
}

