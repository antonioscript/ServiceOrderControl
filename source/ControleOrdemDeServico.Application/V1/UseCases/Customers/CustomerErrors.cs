using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers;

public static class CustomerErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Customer.NameRequired", "Name is required.");

    public static readonly Error NameTooShort =
        Error.Validation("Customer.NameTooShort", "Name must be at least 2 characters.");

    public static readonly Error NameTooLong =
        Error.Validation("Customer.NameTooLong", "Name must be at most 150 characters.");

    public static readonly Error InvalidEmail =
        Error.Validation("Customer.InvalidEmail", "Email is not valid.");

    public static readonly Error EmailTooLong =
        Error.Validation("Customer.EmailTooLong", "Email must be at most 120 characters.");

    public static readonly Error PhoneTooLong =
        Error.Validation("Customer.PhoneTooLong", "Phone must be at most 30 characters.");

    public static readonly Error DocumentTooLong =
        Error.Validation("Customer.DocumentTooLong", "Document must be at most 30 characters.");

    public static readonly Error DocumentAlreadyExists =
        Error.Conflict("Customer.DocumentAlreadyExists", "A customer with this document already exists.");

    public static readonly Error PhoneAlreadyExists =
        Error.Conflict("Customer.PhoneAlreadyExists", "A customer with this phone already exists.");

    public static readonly Error SearchCriteriaRequired =
        Error.Validation("Customer.SearchCriteriaRequired","At least phone or document must be informed.");

    public static readonly Error NotFound =
        Error.NotFound("Customer.NotFound", "Customer not found.");
}
