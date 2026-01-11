using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Domain.ResultPattern;
using System.Net.Mail;
using System.Xml.Linq;

namespace OsService.Application.V1.Features.Customers.CreateCustomer;

public sealed class CreateCustomerHandler(
    ICustomerRepository repo,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var primitiveValidation = ValidatePrimitiveRules(request);
        if (primitiveValidation.IsFailure)
            return Result.Failure<Guid>(primitiveValidation.Error); //TODO: Resolver retorno GUID zerado

        var duplicationValidation = await ValidateDuplicatesAsync(request, repo, ct);
        if (duplicationValidation.IsFailure)
            return Result.Failure<Guid>(duplicationValidation.Error);

        var customer = mapper.Map<CustomerEntity>(request);

        await repo.AddAsync(customer, ct);
        await unitOfWork.CommitAsync(ct);

        return Result.Success(customer.Id);
    }

    private static Result ValidatePrimitiveRules(CreateCustomerCommand request)
    {
        var name = request.Name?.Trim();
        var phone = request.Phone?.Trim();
        var email = request.Email?.Trim();
        var document = request.Document?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(CustomerErrors.NameRequired);

        if (name.Length < 2)
            return Result.Failure(CustomerErrors.NameTooShort);

        if (name.Length > 150)
            return Result.Failure(CustomerErrors.NameTooLong);

        if (!string.IsNullOrWhiteSpace(phone) && phone.Length > 30)
            return Result.Failure(CustomerErrors.PhoneTooLong);

        if (!string.IsNullOrWhiteSpace(document) && document.Length > 30)
            return Result.Failure(CustomerErrors.DocumentTooLong);

        if (!string.IsNullOrWhiteSpace(email))
        {
            if (email.Length > 120)
                return Result.Failure(CustomerErrors.EmailTooLong);

            try
            {
                _ = new MailAddress(email);
            }
            catch
            {
                return Result.Failure(CustomerErrors.InvalidEmail);
            }
        }

        return Result.Success();
    }

    private static async Task<Result> ValidateDuplicatesAsync(
        CreateCustomerCommand request,
        ICustomerRepository repo,
        CancellationToken ct)
    {
        var phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        var document = string.IsNullOrWhiteSpace(request.Document) ? null : request.Document.Trim();

        if (document is not null)
        {
            var existsDoc = await repo.ExistsByDocumentAsync(document, ct);
            if (existsDoc)
                return Result.Failure(CustomerErrors.DocumentAlreadyExists);
        }

        if (phone is not null)
        {
            var existsPhone = await repo.ExistsByPhoneAsync(phone, ct);
            if (existsPhone)
                return Result.Failure(CustomerErrors.PhoneAlreadyExists);
        }

        return Result.Success();
    }

}