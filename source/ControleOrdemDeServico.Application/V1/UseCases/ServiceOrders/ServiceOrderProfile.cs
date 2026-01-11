using AutoMapper;
using OsService.Application.V1.UseCases.Customers.GetCustomerById;
using OsService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static OsService.Application.V1.UseCases.Customers.CreateCustomer.CreateCustomer;

namespace OsService.Application.V1.UseCases.ServiceOrders;

public class ServiceOrderProfile : Profile
{
    public ServiceOrderProfile()
    {
        CreateMap<OpenServiceOrder.OpenServiceOrder., CustomerEntity>();
        CreateMap<CustomerEntity, GetCustomerByContactResponse>();
    }
}
