using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.OpenServiceOrder;

public partial class OpenServiceOrder
{
    private static Command Normalize(Command request)
    {
        var description = request.Description?.Trim() ?? string.Empty;

        string? coin = null;

        if (request.Price is not null)
        {
            coin = string.IsNullOrWhiteSpace(request.Coin)
                ? "BRL"
                : request.Coin.Trim().ToUpperInvariant();
        }

        return request with
        {
            Description = description,
            Coin = coin
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