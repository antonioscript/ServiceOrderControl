using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.OpenServiceOrder;

public partial class OpenServiceOrder
{
	public sealed class OpenServiceOrderHandler(
		IServiceOrderRepository serviceOrders,
		ICustomerRepository customers,
		IUnitOfWork unitOfWork,
		IMapper mapper)
		: IRequestHandler<Command, Result<Response>>
	{
		public async Task<Result<Response>> Handle(
			Command request,
			CancellationToken ct)
		{
			var normalized = Normalize(request);

			var primitive = ValidatePrimitiveRules(normalized);
			if (primitive.IsFailure)
				return Result.Failure<Response>(primitive.Error);

			var customerExists = await customers.ExistsAsync(normalized.CustomerId, ct);
			if (!customerExists)
				return Result.Failure<Response>(ServiceOrderErrors.CustomerNotFound);

			var entity = mapper.Map<ServiceOrderEntity>(normalized);

			//entity.Status = ServiceOrderStatus.Open;
			//entity.OpenedAt = DateTime.UtcNow;

			await serviceOrders.AddAsync(entity, ct);
			await unitOfWork.CommitAsync(ct);

			var response = mapper.Map<Response>(entity);
			return Result.Success(response);
		}
	}
}