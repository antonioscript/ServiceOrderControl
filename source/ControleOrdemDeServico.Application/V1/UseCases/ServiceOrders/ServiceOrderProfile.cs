using AutoMapper;
using OsService.Domain.Entities;
using static OsService.Application.V1.UseCases.ServiceOrders.OpenServiceOrder.OpenServiceOrder;

namespace OsService.Application.V1.UseCases.ServiceOrders;

public class ServiceOrderProfile : Profile
{
    public ServiceOrderProfile()
    {
        // Command 
        CreateMap<Command, ServiceOrderEntity>();

        // Entity
        CreateMap<ServiceOrderEntity, Response>();
        CreateMap<ServiceOrderEntity, GetServiceOrderById.GetServiceOrderById.Response>();
        CreateMap<ServiceOrderEntity, SearchServiceOrders.SearchServiceOrders.Response>();
        CreateMap<ServiceOrderEntity, ChangeServiceOrderStatus.ChangeServiceOrderStatus.Response>();
        CreateMap<ServiceOrderEntity, UpdateServiceOrderPrice.UpdateServiceOrderPrice.Response>();
    }
}
