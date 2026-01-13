using AutoMapper;
using OsService.Application.V1.UseCases.Customers.GetCustomerById;
using OsService.Domain.Entities;
using static OsService.Application.V1.UseCases.Customers.CreateCustomer.CreateCustomer;

namespace OsService.Application.V1.UseCases.Customers;

public sealed class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<CreateCustomerCommand, CustomerEntity>();
        CreateMap<CustomerEntity, GetCustomerByContact.GetCustomerByContact.Response>();
        CreateMap<CustomerEntity, GetCustomerById.GetCustomerById.Response>();
    }
}