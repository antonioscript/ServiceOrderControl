using MediatR;
using OsService.Domain.ResultPattern;
using System.Text.Json.Serialization;

namespace OsService.Application.V1.UseCases.ServiceOrders.UpdateServiceOrderPrice;

public partial class UpdateServiceOrderPrice
{
    public sealed record UpdateServiceOrderPriceCommand : IRequest<Result<Response>>
    {
        [JsonIgnore]                
        public Guid Id { get; init; }

        public decimal? Price { get; init; }

        public string? Coin { get; init; }
    }
}