using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.SearchServiceOrders;

public partial class SearchServiceOrders
{
    public sealed class Handler(
        IServiceOrderRepository serviceOrders,
        IMapper mapper)
        : IRequestHandler<Query, Result<IReadOnlyList<Response>>>
    {
        public async Task<Result<IReadOnlyList<Response>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var validation = ValidatePrimitiveRules(request);
            if (validation.IsFailure)
                return Result.Failure<IReadOnlyList<Response>>(validation.Error);

            DateTime? start = request.StartDate?.Date;                 
            DateTime? endExclusive = request.EndDate?.Date.AddDays(1);

            var list = await serviceOrders.ListAsync(so =>
                    (!request.CustomerId.HasValue || so.CustomerId == request.CustomerId.Value) &&
                    (!request.Status.HasValue || so.Status == request.Status.Value) &&
                    (!start.HasValue || so.OpenedAt >= start.Value) &&
                    (!endExclusive.HasValue || so.OpenedAt < endExclusive.Value),
                cancellationToken);

            var response = mapper.Map<IReadOnlyList<Response>>(list);
            return Result.Success(response);
        }
    }
}