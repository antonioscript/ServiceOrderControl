using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.SearchServiceOrders;

public partial class SearchServiceOrders
{
    private static Result ValidatePrimitiveRules(Query request)
    {
        if (!request.CustomerId.HasValue &&
            !request.Status.HasValue &&
            !request.StartDate.HasValue &&
            !request.EndDate.HasValue)
        {
            return Result.Failure(ServiceOrderErrors.SearchCriteriaRequired);
        }

        if (request.StartDate.HasValue && request.EndDate.HasValue &&
            request.StartDate.Value > request.EndDate.Value)
        {
            return Result.Failure(ServiceOrderErrors.InvalidPeriod);
        }

        return Result.Success();
    }
}
