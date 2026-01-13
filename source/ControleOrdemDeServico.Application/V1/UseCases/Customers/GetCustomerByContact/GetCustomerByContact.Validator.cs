using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers.GetCustomerByContact;

public partial class GetCustomerByContact
{
    public static Query Normalize(Query request)
    {
        return request with
        {
            Phone = string.IsNullOrWhiteSpace(request.Phone)
                ? null
                : request.Phone.Trim(),
            Document = string.IsNullOrWhiteSpace(request.Document)
                ? null
                : request.Document.Trim()
        };
    }

    public static Result ValidatePrimitiveRules(Query request)
    {
        if (request.Phone is null && request.Document is null)
            return Result.Failure(CustomerErrors.SearchCriteriaRequired);

        if (request.Phone is not null && request.Phone.Length > 30)
            return Result.Failure(CustomerErrors.PhoneTooLong);

        if (request.Document is not null && request.Document.Length > 30)
            return Result.Failure(CustomerErrors.DocumentTooLong);

        return Result.Success();
    }

    public static async Task<CustomerEntity?> FindByContactAsync(
        Query request,
        ICustomerRepository repo,
        CancellationToken ct)
    {
        if (request.Document is not null)
        {
            var list = await repo.ListAsync(c => c.Document == request.Document, ct);
            return list.FirstOrDefault();
        }

        if (request.Phone is not null)
        {
            var list = await repo.ListAsync(c => c.Phone == request.Phone, ct);
            return list.FirstOrDefault();
        }

        return null;
    }
}
