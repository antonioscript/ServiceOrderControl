using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers.GetCustomerByContact;

public partial class GetCustomerByContact 
{
    public sealed class Handler(
        ICustomerRepository repo,
        IMapper mapper
)
    : IRequestHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var normalized = Normalize(request);

            var validation = ValidatePrimitiveRules(normalized);
            if (validation.IsFailure)
                return Result.Failure<Response>(validation.Error);

            var entity = await FindByContactAsync(normalized, repo, cancellationToken);

            if (entity is null)
                return Result.Failure<Response>(CustomerErrors.NotFound);

            var response = mapper.Map<Response>(entity);
            return Result.Success(response);
        }
    }

}

