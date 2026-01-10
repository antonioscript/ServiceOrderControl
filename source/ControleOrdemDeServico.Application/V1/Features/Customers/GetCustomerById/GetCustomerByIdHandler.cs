using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;

namespace OsService.Application.V1.Features.Customers.GetCustomerById;

public sealed class GetCustomerByIdHandler(
    ICustomerRepository repo,
    IMapper mapper)
    : IRequestHandler<GetCustomerByIdQuery, GetCustomerByIdResponse?>
{
    public async Task<GetCustomerByIdResponse?> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(request.Id, ct);
        return entity is null
            ? null
            : mapper.Map<GetCustomerByIdResponse>(entity);
    }
}
