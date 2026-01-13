using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers.GetCustomerById;

//TODO: Transformar em UseCase
public sealed class GetCustomerByIdHandler(
    ICustomerRepository repo,
    IMapper mapper)
    : IRequestHandler<GetCustomerByIdQuery, Result<GetCustomerByContactResponse>>
{
    public async Task<Result<GetCustomerByContactResponse>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repo.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            return Result.Failure<GetCustomerByContactResponse>(CustomerErrors.NotFound);

        var response = mapper.Map<GetCustomerByContactResponse>(entity);
        return Result.Success(response);
    }
}
