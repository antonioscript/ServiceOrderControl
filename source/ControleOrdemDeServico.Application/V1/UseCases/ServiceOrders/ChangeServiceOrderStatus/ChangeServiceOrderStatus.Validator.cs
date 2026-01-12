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

        // Em geral nem precisa disso, mas deixa claro:
        if (!Enum.IsDefined(typeof(ServiceOrderStatus), request.NewStatus))
            return Result.Failure(ServiceOrderErrors.InvalidStatusTransition);

        return Result.Success();
    }

    private static Result ValidateTransition(ServiceOrderEntity entity, ServiceOrderStatus newStatus)
    {
        var current = entity.Status;

        // já finalizada -> bloqueado (qualquer coisa)
        if (current == ServiceOrderStatus.Finished)
            return Result.Failure(ServiceOrderErrors.AlreadyFinished);

        // Aberta -> Em Execução (permitido)
        if (current == ServiceOrderStatus.Open &&
            newStatus == ServiceOrderStatus.InProgress)
        {
            return Result.Success();
        }

        // Em Execução -> Finalizada (permitido, mas checa regra 2.4)
        if (current == ServiceOrderStatus.InProgress &&
            newStatus == ServiceOrderStatus.Finished)
        {
            if (entity.Price is null || entity.Price < 0)
                return Result.Failure(ServiceOrderErrors.PriceRequiredToFinish);

            return Result.Success();
        }

        // Aberta -> Finalizada (bloqueado)
        // Em Execução -> Aberta (bloqueado)
        // qualquer outro salto bizarro
        return Result.Failure(ServiceOrderErrors.InvalidStatusTransition);
    }
}