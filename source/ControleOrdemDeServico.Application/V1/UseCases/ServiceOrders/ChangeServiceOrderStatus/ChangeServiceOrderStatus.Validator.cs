using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.ChangeServiceOrderStatus;

public partial class ChangeServiceOrderStatus
{
    private static Result ValidatePrimitiveRules(ChangeServiceOrderCommand request)
    {
        if (request.Id == Guid.Empty)
            return Result.Failure(ServiceOrderErrors.IdRequired);

        if (!Enum.IsDefined(typeof(ServiceOrderStatus), request.NewStatus))
            return Result.Failure(ServiceOrderErrors.InvalidStatusTransition);

        return Result.Success();
    }

    private static Result ValidateTransition(ServiceOrderEntity entity, ServiceOrderStatus newStatus)
    {
        var current = entity.Status;

        if (current == ServiceOrderStatus.Finished)
            return Result.Failure(ServiceOrderErrors.AlreadyFinished);

        if (current == ServiceOrderStatus.Open &&
            newStatus == ServiceOrderStatus.InProgress)
        {
            return Result.Success();
        }

        if (current == ServiceOrderStatus.InProgress &&
            newStatus == ServiceOrderStatus.Finished)
        {
            if (entity.Price is null || entity.Price < 0)
                return Result.Failure(ServiceOrderErrors.PriceRequiredToFinish);

            return Result.Success();
        }

        return Result.Failure(ServiceOrderErrors.InvalidStatusTransition);
    }
}