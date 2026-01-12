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

    public static readonly Error IdRequired =
        Error.Validation("ServiceOrder.IdRequired", "Service order id is required.");

    public static readonly Error SearchCriteriaRequired =
        Error.Validation("ServiceOrder.SearchCriteriaRequired","At least one filter (customer, status, or period) must be provided.");

    public static readonly Error InvalidPeriod =
        Error.Validation("ServiceOrder.InvalidPeriod","The initial date cannot be greater than the final date.");

    public static readonly Error InvalidStatusTransition =
        Error.Conflict("ServiceOrder.InvalidStatusTransition","This status transition is not allowed.");

    public static readonly Error AlreadyFinished =
        Error.Conflict("ServiceOrder.AlreadyFinished","A finished service order cannot change status.");

    public static readonly Error PriceRequiredToFinish =
        Error.Validation("ServiceOrder.PriceRequiredToFinish", "A service order must have a value to be finished.");

    public static readonly Error PriceChangeNotAllowed =
        Error.Conflict("ServiceOrder.PriceChangeNotAllowed", "Price cannot be changed after the service order is finished.");


}