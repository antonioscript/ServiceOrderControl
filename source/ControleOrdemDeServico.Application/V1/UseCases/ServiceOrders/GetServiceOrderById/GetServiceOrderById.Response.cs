namespace OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderById;

public partial class GetServiceOrderById
{
    public sealed record Response(
        Guid Id,
        int Number,
        Guid CustomerId,
        string Description,
        string Status,
        DateTime OpenedAt,
        decimal? Price,
        string? Coin,
        DateTime? UpdatedPriceAt
    );
}
