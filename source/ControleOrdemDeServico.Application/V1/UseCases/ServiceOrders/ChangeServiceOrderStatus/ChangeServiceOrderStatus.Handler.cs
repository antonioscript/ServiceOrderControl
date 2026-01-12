using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Enums;
using OsService.Domain.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsService.Application.V1.UseCases.ServiceOrders.ChangeServiceOrderStatus;

public partial class ChangeServiceOrderStatus
{
    public sealed class Handler(
        IServiceOrderRepository serviceOrders,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : IRequestHandler<ChangeServiceOrderCommand, Result<Response>>
    {
        public async Task<Result<Response>> Handle(
            ChangeServiceOrderCommand request,
            CancellationToken ct)
        {
            // 1) Validação básica
            var primitive = ValidatePrimitiveRules(request);
            if (primitive.IsFailure)
                return Result.Failure<Response>(primitive.Error);

            // 2) Carrega OS
            var entity = await serviceOrders.GetByIdAsync(request.Id, ct);
            if (entity is null)
                return Result.Failure<Response>(ServiceOrderErrors.NotFound);

            // 3) Valida transição de status (inclui regra 2.4 para finalizar)
            var transition = ValidateTransition(entity, request.NewStatus);
            if (transition.IsFailure)
                return Result.Failure<Response>(transition.Error);

            var now = DateTime.UtcNow;

            // 4) Atualiza datas de ciclo
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

            // 5) Atualiza status
            entity.Status = request.NewStatus;

            //await serviceOrders.UpdateAsync(entity, ct);
            await unitOfWork.CommitAsync(ct);

            var response = mapper.Map<Response>(entity);
            return Result.Success(response);
        }
    }
}