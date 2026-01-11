using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders;

public static class ServiceOrderErrors
{
    public static readonly Error CustomerRequired =
        Error.Validation("ServiceOrder.CustomerRequired", "CustomerId is required.");

    public static readonly Error DescriptionRequired =
        Error.Validation("ServiceOrder.DescriptionRequired", "Description is required.");

    public static readonly Error DescriptionTooLong =
        Error.Validation("ServiceOrder.DescriptionTooLong", "Description must be at most 500 characters.");

    public static readonly Error PriceNegative =
        Error.Validation("ServiceOrder.PriceNegative", "Price cannot be negative.");

    public static readonly Error CustomerNotFound =
        Error.NotFound("ServiceOrder.CustomerNotFound", "Customer not found.");

    public static readonly Error NotFound =
        Error.NotFound("ServiceOrder.NotFound", "Service order not found.");
}