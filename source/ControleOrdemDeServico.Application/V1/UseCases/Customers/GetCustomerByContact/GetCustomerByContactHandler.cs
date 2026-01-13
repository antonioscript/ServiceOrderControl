using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Application.V1.UseCases.Customers.GetCustomerById;
using OsService.Domain.Entities;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers.GetCustomerByContact;

//TODO: Transformar em UseCase
public sealed class GetCustomerByContactHandler(
    ICustomerRepository repo,
    IMapper mapper
)
    : IRequestHandler<GetCustomerByContactQuery, Result<GetCustomerByContactResponse>>
{
    public async Task<Result<GetCustomerByContactResponse>> Handle(
        GetCustomerByContactQuery request,
        CancellationToken cancellationToken)
    {
        var normalized = Normalize(request);

        var validation = ValidatePrimitiveRules(normalized);
        if (validation.IsFailure)
            return Result.Failure<GetCustomerByContactResponse>(validation.Error);

        var entity = await FindByContactAsync(normalized, repo, cancellationToken);

        if (entity is null)
            return Result.Failure<GetCustomerByContactResponse>(CustomerErrors.NotFound);

        var response = mapper.Map<GetCustomerByContactResponse>(entity);
        return Result.Success(response);
    }

    private static GetCustomerByContactQuery Normalize(GetCustomerByContactQuery request)
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

    private static Result ValidatePrimitiveRules(GetCustomerByContactQuery request)
    {
        if (request.Phone is null && request.Document is null)
            return Result.Failure(CustomerErrors.SearchCriteriaRequired);

        if (request.Phone is not null && request.Phone.Length > 30)
            return Result.Failure(CustomerErrors.PhoneTooLong);

        if (request.Document is not null && request.Document.Length > 30)
            return Result.Failure(CustomerErrors.DocumentTooLong);

        return Result.Success();
    }

    private static async Task<CustomerEntity?> FindByContactAsync(
        GetCustomerByContactQuery request,
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
