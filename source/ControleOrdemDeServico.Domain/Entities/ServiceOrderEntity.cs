using OsService.Domain.Common;
using OsService.Domain.Enums;

namespace OsService.Domain.Entities;

public sealed class ServiceOrderEntity : BaseEntity
{
    public int Number { get; init; } 
    public Guid CustomerId { get; init; }
    public string Description { get; init; } = default!;
    public ServiceOrderStatus Status { get; set; } = ServiceOrderStatus.Open;
    public DateTime OpenedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    public decimal? Price { get; set; }
    public string? Coin { get; set; } = "BRL"; 
    public DateTime? UpdatedPriceAt { get; set; }

}
