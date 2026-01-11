using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers.CreateCustomer;

public partial class CreateCustomer
{
	public sealed class CreateCustomerHandler(
		ICustomerRepository repo,
		IUnitOfWork unitOfWork,
		IMapper mapper)
		: IRequestHandler<CreateCustomerCommand, Result<Guid>>
	{
		public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken ct)
		{
			var normalized = Normalize(request);

			var primitiveValidation = ValidatePrimitiveRules(normalized);
			if (primitiveValidation.IsFailure)
				return Result.Failure<Guid>(primitiveValidation.Error);

			var duplicationValidation = await ValidateDuplicatesAsync(normalized, repo, ct);
			if (duplicationValidation.IsFailure)
				return Result.Failure<Guid>(duplicationValidation.Error);

			var customer = mapper.Map<CustomerEntity>(normalized);

			await repo.AddAsync(customer, ct);
			await unitOfWork.CommitAsync(ct);

			return Result.Success(customer.Id);
		}
	}
}