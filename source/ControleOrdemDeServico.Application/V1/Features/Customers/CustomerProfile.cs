using AutoMapper;
using OsService.Application.V1.Features.Customers.CreateCustomer;
using OsService.Application.V1.Features.Customers.GetCustomerById;
using OsService.Domain.Entities;

namespace OsService.Application.V1.Features.Customers;

public sealed class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<CreateCustomerCommand, CustomerEntity>();
        CreateMap<CustomerEntity, GetCustomerByIdResponse>();
    }
}