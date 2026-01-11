using OsService.Domain.ResultPattern;


namespace OsService.Application.V1.Features.Customers;

//TODO: Avaliar melhor lugar de acordo com a arquitetura
public static class CustomerErrors
{
    public static readonly Error NameRequired =
        new("Customer.NameRequired", "Name is required.");

    public static readonly Error NameTooLong =
        new("Customer.NameTooLong", "Name must be <= 150 characters.");

    public static readonly Error NotFound =
        new("Customer.NotFound", "Customer not found.");
}