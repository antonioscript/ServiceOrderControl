using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderById;

public partial class GetServiceOrderById
{
    public sealed class Handler(
        IServiceOrderRepository serviceOrders,
        IMapper mapper)
        : IRequestHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                return Result.Failure<Response>(ServiceOrderErrors.IdRequired);

            ServiceOrderEntity? entity = await serviceOrders.GetByIdAsync(request.Id, cancellationToken);

            if (entity is null)
                return Result.Failure<Response>(ServiceOrderErrors.NotFound);

            var response = mapper.Map<Response>(entity);
            return Result.Success(response);
        }
    }
}