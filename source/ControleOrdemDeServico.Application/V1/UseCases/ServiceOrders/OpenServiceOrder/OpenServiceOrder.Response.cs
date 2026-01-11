namespace OsService.Application.V1.UseCases.ServiceOrders.OpenServiceOrder;

public partial class OpenServiceOrder
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
