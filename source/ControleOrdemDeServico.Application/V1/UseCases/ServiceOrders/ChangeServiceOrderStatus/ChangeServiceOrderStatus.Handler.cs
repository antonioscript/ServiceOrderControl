using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Enums;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.ChangeServiceOrderStatus;

public partial class ChangeServiceOrderStatus
{
    public sealed class Handler(
        IServiceOrderRepository serviceOrders,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<Handler> logger)
        : IRequestHandler<ChangeServiceOrderCommand, Result<Response>>
    {
        public async Task<Result<Response>> Handle(
            ChangeServiceOrderCommand request,
            CancellationToken ct)
        {
            logger.LogInformation(
                "Iniciando mudança de status da OS. ServiceOrderId={ServiceOrderId}, NewStatus={NewStatus}",
                request.Id,
                request.NewStatus);

            var primitive = ValidatePrimitiveRules(request);
            if (primitive.IsFailure)
            {
                logger.LogWarning(
                    "Falha de validação ao mudar status da OS. ServiceOrderId={ServiceOrderId}, Code={Code}, Message={Message}",
                    request.Id,
                    primitive.Error.Code,
                    primitive.Error.Message);

                return Result.Failure<Response>(primitive.Error);
            }
                

            var entity = await serviceOrders.GetByIdAsync(request.Id, ct);
            if (entity is null)
            {
                logger.LogWarning(
                    "Tentativa de mudar status de OS inexistente. ServiceOrderId={ServiceOrderId}",
                    request.Id);

                return Result.Failure<Response>(ServiceOrderErrors.NotFound);
            }
                

            var transition = ValidateTransition(entity, request.NewStatus);
            if (transition.IsFailure)
            {
                logger.LogWarning(
                    "Transição de status inválida. ServiceOrderId={ServiceOrderId}, From={From}, To={To}, Code={Code}",
                    entity.Id,
                    entity.Status,
                    request.NewStatus,
                    transition.Error.Code);

                return Result.Failure<Response>(transition.Error);
            }
                

            var now = DateTime.UtcNow;

            if (entity.Status == ServiceOrderStatus.Open &&
                request.NewStatus == ServiceOrderStatus.InProgress &&
                entity.StartedAt is null)
            {
                entity.StartedAt = now;
            }

            if (entity.Status == ServiceOrderStatus.InProgress &&
                request.NewStatus == ServiceOrderStatus.Finished &&
                entity.FinishedAt is null)
            {
                entity.FinishedAt = now;
            }

            entity.Status = request.NewStatus;

            await unitOfWork.CommitAsync(ct);

            logger.LogInformation(
                "Status da OS atualizado com sucesso. ServiceOrderId={ServiceOrderId}, NewStatus={NewStatus}",
                entity.Id,
                entity.Status);

            var response = mapper.Map<Response>(entity);
            return Result.Success(response);
        }
    }
}