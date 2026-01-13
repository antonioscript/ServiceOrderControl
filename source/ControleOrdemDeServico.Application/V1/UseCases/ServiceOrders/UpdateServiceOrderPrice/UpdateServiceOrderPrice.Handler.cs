using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Enums;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.UpdateServiceOrderPrice;

public partial class UpdateServiceOrderPrice
{
    public sealed class Handler(
        IServiceOrderRepository serviceOrders,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : IRequestHandler<UpdateServiceOrderPriceCommand, Result<Response>>
    {
        public async Task<Result<Response>> Handle(
            UpdateServiceOrderPriceCommand request,
            CancellationToken cancellationToken)
        {
            var normalized = Normalize(request);

            var primitive = ValidatePrimitiveRules(normalized);
            if (primitive.IsFailure)
                return Result.Failure<Response>(primitive.Error);

            var entity = await serviceOrders.GetByIdAsync(normalized.Id, cancellationToken);
            if (entity is null)
                return Result.Failure<Response>(ServiceOrderErrors.NotFound);

            if (entity.Status == ServiceOrderStatus.Finished)
                return Result.Failure<Response>(ServiceOrderErrors.PriceChangeNotAllowed);

            entity.Price = normalized.Price;
            entity.Coin = normalized.Price is null
                ? null
                : normalized.Coin ?? "BRL"; //TODO

            entity.UpdatedPriceAt = DateTime.UtcNow;

            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<Response>(entity);
            return Result.Success(response);
        }
    }
}