using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.Features.Customers;

public static class CustomerErrors
{
    public static readonly Error NameRequired =
        new("Customer.NameRequired", "Name is required.");

    public static readonly Error NameTooShort =
        new("Customer.NameTooShort", "Name must be at least 2 characters.");

    public static readonly Error NameTooLong =
        new("Customer.NameTooLong", "Name must be at most 150 characters.");

    public static readonly Error InvalidEmail =
        new("Customer.InvalidEmail", "Email is not valid.");

    public static readonly Error EmailTooLong =
        new("Customer.EmailTooLong", "Email must be at most 120 characters.");

    public static readonly Error PhoneTooLong =
        new("Customer.PhoneTooLong", "Phone must be at most 30 characters.");

    public static readonly Error DocumentTooLong =
        new("Customer.DocumentTooLong", "Document must be at most 30 characters.");

    public static readonly Error NotFound =
        new("Customer.NotFound", "Customer not found.");

    public static readonly Error DocumentAlreadyExists =
        new("Customer.DocumentAlreadyExists", "A customer with this document already exists.");

    public static readonly Error PhoneAlreadyExists =
        new("Customer.PhoneAlreadyExists", "A customer with this phone already exists.");
}