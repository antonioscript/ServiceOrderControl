using OsService.Domain.Common;
using OsService.Domain.Enums;

namespace OsService.Domain.Entities;

//TODO: Verificar como deve ser o preenchimeno automático, via Handler ou aqui dentro
//Tô começando a pensar em deixar dentro do Handler por falar de regra de negócio...
public sealed class ServiceOrderEntity : BaseEntity
{
    public int Number { get; init; } //Gerado automaticamente
    public Guid CustomerId { get; init; }
    public string Description { get; init; } = default!;
    public ServiceOrderStatus Status { get; init; } = ServiceOrderStatus.Open;
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    public decimal? Price { get; init; }
    public string? Coin { get; init; } = "BRL";
    public DateTime? UpdatedPriceAt { get; init; }

}
