using AutoMapper;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;

namespace OsService.Application.V1.Features.Customers.CreateCustomer;

public sealed class CreateCustomerHandler(
    ICustomerRepository repo,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        ValidateRequest(request);

        var customer = mapper.Map<CustomerEntity>(request);

        await repo.AddAsync(customer, ct);
        await unitOfWork.CommitAsync(ct);


        //Todo: Pensar no Response
        return customer.Id;
    }

    private static void ValidateRequest(CreateCustomerCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required.", nameof(request.Name));

        if (request.Name.Length > 150)
            throw new ArgumentException("Name must be <= 150 characters.", nameof(request.Name));
    }
}