using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.Features.Customers.CreateCustomer;

public sealed class CreateCustomerHandler(
    ICustomerRepository repo,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var validationResult = ValidateRequest(request);
        if (validationResult.IsFailure)
            return Result.Failure<Guid>(validationResult.Error);

        var customer = mapper.Map<CustomerEntity>(request);

        await repo.AddAsync(customer, ct);
        await unitOfWork.CommitAsync(ct);

        return Result.Success(customer.Id);
    }

    private static Result ValidateRequest(CreateCustomerCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result.Failure(CustomerErrors.NameRequired);

        if (request.Name.Length > 150)
            return Result.Failure(CustomerErrors.NameTooLong);

        return Result.Success();
    }
}