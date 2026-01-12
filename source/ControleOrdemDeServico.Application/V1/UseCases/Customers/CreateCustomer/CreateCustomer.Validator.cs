using System.Net.Mail;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers.CreateCustomer;

public partial class CreateCustomer
{
    private static CreateCustomerCommand Normalize(CreateCustomerCommand request)
    {
        return request with
        {
            Name = request.Name.Trim(),
            Phone = request.Phone?.Trim(),
            Email = request.Email?.Trim(),
            Document = request.Document?.Trim()
        };
    }

    private static Result ValidatePrimitiveRules(CreateCustomerCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result.Failure(CustomerErrors.NameRequired);

        if (request.Name.Length < 2)
            return Result.Failure(CustomerErrors.NameTooShort);

        if (request.Name.Length > 150)
            return Result.Failure(CustomerErrors.NameTooLong);

        if (!string.IsNullOrWhiteSpace(request.Phone) && request.Phone.Length > 30)
            return Result.Failure(CustomerErrors.PhoneTooLong);

        if (!string.IsNullOrWhiteSpace(request.Document) && request.Document.Length > 30)
            return Result.Failure(CustomerErrors.DocumentTooLong);

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            if (request.Email.Length > 120)
                return Result.Failure(CustomerErrors.EmailTooLong);

            try
            {
                _ = new MailAddress(request.Email);
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
        if (request.Document is not null)
        {
            var existsDoc = await repo.ExistsByDocumentAsync(request.Document, ct);
            if (existsDoc)
                return Result.Failure(CustomerErrors.DocumentAlreadyExists);
        }

        if (request.Phone is not null)
        {
            var existsPhone = await repo.ExistsByPhoneAsync(request.Phone, ct);
            if (existsPhone)
                return Result.Failure(CustomerErrors.PhoneAlreadyExists);
        }

        return Result.Success();
    }
}
