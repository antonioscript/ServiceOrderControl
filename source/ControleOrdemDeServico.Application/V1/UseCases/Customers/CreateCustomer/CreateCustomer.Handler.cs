using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers.CreateCustomer;

public partial class CreateCustomer
{
	public sealed class CreateCustomerHandler(
		ICustomerRepository repo,
		IUnitOfWork unitOfWork,
		IMapper mapper,
        ILogger<CreateCustomerHandler> logger)
		: IRequestHandler<CreateCustomerCommand, Result<Guid>>
	{
		public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken ct)
		{
            logger.LogInformation(
                "Iniciando criação de cliente. Name={Name}, Phone={Phone}, Email={Email}, Document={Document}",
                request.Name, request.Phone, request.Email, request.Document);

            var normalized = Normalize(request);

			var primitiveValidation = ValidatePrimitiveRules(normalized);
			if (primitiveValidation.IsFailure)
			{
                logger.LogWarning(
                    "Falha de validação ao criar cliente. Code={Code}, Message={Message}",
                    primitiveValidation.Error.Code,
                    primitiveValidation.Error.Message);

                return Result.Failure<Guid>(primitiveValidation.Error);
            }
				

			var duplicationValidation = await ValidateDuplicatesAsync(normalized, repo, ct);
			if (duplicationValidation.IsFailure)
			{
                logger.LogWarning(
                    "Falha de duplicidade ao criar cliente. Code={Code}, Message={Message}",
                    duplicationValidation.Error.Code,
                    duplicationValidation.Error.Message);

				return Result.Failure<Guid>(duplicationValidation.Error);
            }
				

			var customer = mapper.Map<CustomerEntity>(normalized);

			await repo.AddAsync(customer, ct);
			await unitOfWork.CommitAsync(ct);

            logger.LogInformation(
                "Cliente criado com sucesso. CustomerId={CustomerId}",
                customer.Id);

            return Result.Success(customer.Id);
		}
	}
}