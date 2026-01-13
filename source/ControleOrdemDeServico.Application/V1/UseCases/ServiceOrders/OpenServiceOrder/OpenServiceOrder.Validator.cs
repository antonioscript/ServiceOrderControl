using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.OpenServiceOrder;

public partial class OpenServiceOrder
{
    private static Command Normalize(Command request)
    {
        var description = request.Description?.Trim() ?? string.Empty;

        return request with
        {
            Description = description
        };
    }

    private static Result ValidatePrimitiveRules(Command request)
    {
        if (request.CustomerId == Guid.Empty)
            return Result.Failure(ServiceOrderErrors.CustomerRequired);

        if (string.IsNullOrWhiteSpace(request.Description))
            return Result.Failure(ServiceOrderErrors.DescriptionRequired);

        if (request.Description.Length > 500)
            return Result.Failure(ServiceOrderErrors.DescriptionTooLong);

        if (request.Price is < 0)
            return Result.Failure(ServiceOrderErrors.PriceNegative);

        return Result.Success();
    }
}