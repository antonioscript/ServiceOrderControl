using OsService.Domain.Enums;

namespace OsService.Application.V1.UseCases.ServiceOrders.ChangeServiceOrderStatus;

public partial class ChangeServiceOrderStatus
{
    public sealed record Response(
        Guid Id,
        int Number,
        ServiceOrderStatus Status,
        DateTime OpenedAt,
        DateTime? StartedAt,
        DateTime? FinishedAt,
        decimal? Price
    );
}
