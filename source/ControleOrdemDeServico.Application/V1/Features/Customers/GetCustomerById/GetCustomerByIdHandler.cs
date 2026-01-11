using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.Features.Customers.GetCustomerById;

public sealed class GetCustomerByIdHandler(
    ICustomerRepository repo,
    IMapper mapper)
    : IRequestHandler<GetCustomerByIdQuery, Result<GetCustomerByIdResponse>>
{
    public async Task<Result<GetCustomerByIdResponse>> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(request.Id, ct);

        if (entity is null)
            return Result.Failure<GetCustomerByIdResponse>(CustomerErrors.NotFound);

        var response = mapper.Map<GetCustomerByIdResponse>(entity);
        return Result.Success(response);
    }
}
