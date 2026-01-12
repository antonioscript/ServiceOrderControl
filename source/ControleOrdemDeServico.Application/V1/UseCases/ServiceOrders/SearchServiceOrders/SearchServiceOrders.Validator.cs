using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.SearchServiceOrders;

public partial class SearchServiceOrders
{
    private static Result ValidatePrimitiveRules(Query request)
    {
        // Pelo menos um filtro tem que vir preenchido
        if (!request.CustomerId.HasValue &&
            !request.Status.HasValue &&
            !request.From.HasValue &&
            !request.To.HasValue)
        {
            return Result.Failure(ServiceOrderErrors.SearchCriteriaRequired);
        }

        // Se veio período completo, from <= to
        if (request.From.HasValue && request.To.HasValue &&
            request.From.Value > request.To.Value)
        {
            return Result.Failure(ServiceOrderErrors.InvalidPeriod);
        }

        return Result.Success();
    }
}
