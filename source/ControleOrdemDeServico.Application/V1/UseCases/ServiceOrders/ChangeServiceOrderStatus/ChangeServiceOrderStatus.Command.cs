using MediatR;
using OsService.Domain.Enums;
using OsService.Domain.ResultPattern;
using System.Text.Json.Serialization;

namespace OsService.Application.V1.UseCases.ServiceOrders.ChangeServiceOrderStatus;

public partial class ChangeServiceOrderStatus
{
    public sealed record ChangeServiceOrderCommand : IRequest<Result<Response>>
    {
        [JsonIgnore]
        public Guid Id { get; init; }
        public ServiceOrderStatus NewStatus { get; init; }
    }
}
