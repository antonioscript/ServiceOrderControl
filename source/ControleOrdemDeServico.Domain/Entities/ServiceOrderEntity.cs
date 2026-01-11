using OsService.Domain.Common;
using OsService.Domain.Enums;

namespace OsService.Domain.Entities;

public sealed class ServiceOrderEntity : BaseEntity
{
    public int Number { get; init; } 
    public Guid CustomerId { get; init; }
    public string Description { get; init; } = default!;
    public ServiceOrderStatus Status { get; set; } 
    public DateTime OpenedAt { get; set; }
    public decimal? Price { get; init; }
    public string? Coin { get; init; } = "BRL";
    public DateTime? UpdatedPriceAt { get; init; }

}
