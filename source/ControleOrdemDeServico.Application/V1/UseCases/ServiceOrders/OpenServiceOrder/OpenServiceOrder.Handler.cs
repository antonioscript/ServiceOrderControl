using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.OpenServiceOrder;

public partial class OpenServiceOrder
{
	public sealed class OpenServiceOrderHandler( //TODO: Nome Handler
		IServiceOrderRepository serviceOrders,
		ICustomerRepository customers,
		IUnitOfWork unitOfWork,
		IMapper mapper,
        ILogger<OpenServiceOrderHandler> logger)
		: IRequestHandler<Command, Result<Response>>
	{
		public async Task<Result<Response>> Handle(
			Command request,
			CancellationToken cancellationToken)
		{
            logger.LogInformation(
                "Iniciando abertura de OS. CustomerId={CustomerId}, Price={Price}",
                request.CustomerId,
                request.Price);

            var normalized = Normalize(request);

			var primitive = ValidatePrimitiveRules(normalized);
			if (primitive.IsFailure)
			{
                logger.LogWarning(
                    "Falha de validação ao abrir OS. Code={Code}, Message={Message}",
                    primitive.Error.Code,
                    primitive.Error.Message);

                return Result.Failure<Response>(primitive.Error);
            }
				

			var customerExists = await customers.ExistsAsync(normalized.CustomerId, cancellationToken);
			if (!customerExists)
			{
                logger.LogWarning(
                    "Tentativa de abrir OS para cliente inexistente. CustomerId={CustomerId}",
                    normalized.CustomerId);

                return Result.Failure<Response>(ServiceOrderErrors.CustomerNotFound);
            }
				

			var entity = mapper.Map<ServiceOrderEntity>(normalized);

			//TODO
			//entity.Status = ServiceOrderStatus.Open;
			//entity.OpenedAt = DateTime.UtcNow;

			await serviceOrders.AddAsync(entity, cancellationToken);
			await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation(
                "OS aberta com sucesso. ServiceOrderId={ServiceOrderId}, Number={Number}, CustomerId={CustomerId}",
                entity.Id,
                entity.Number,
                entity.CustomerId);

            var response = mapper.Map<Response>(entity);
			return Result.Success(response);
		}
	}
}