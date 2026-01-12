using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.UpdateServiceOrderPrice;

public partial class UpdateServiceOrderPrice
{
    private static UpdateServiceOrderPriceCommand Normalize(UpdateServiceOrderPriceCommand request)
    {
        // Se não tem preço -> não faz sentido moeda
        if (request.Price is null)
            return request with { Coin = null };

        var coin = string.IsNullOrWhiteSpace(request.Coin)
            ? "BRL"
            : request.Coin.Trim().ToUpperInvariant();

        return request with { Coin = coin };
    }

    private static Result ValidatePrimitiveRules(UpdateServiceOrderPriceCommand request)
    {
        if (request.Id == Guid.Empty)
            return Result.Failure(ServiceOrderErrors.IdRequired); // se não tiver esse erro, pode trocar por um genérico

        if (request.Price is < 0)
            return Result.Failure(ServiceOrderErrors.PriceNegative);

        return Result.Success();
    }
}
