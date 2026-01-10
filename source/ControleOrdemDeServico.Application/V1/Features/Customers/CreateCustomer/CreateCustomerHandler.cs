using OsService.Domain.Entities;
using MediatR;
using OsService.Application.V1.Abstractions.Persistence;

namespace OsService.Application.V1.Features.Customers.CreateCustomer;

public sealed class CreateCustomerHandler(ICustomerRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required.");

        var customer = new CustomerEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Phone = request.Phone?.Trim(),
            Email = request.Email?.Trim(),
            Document = request.Document?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(customer, ct);
        await unitOfWork.CommitAsync(ct);

        return customer.Id;
    }
}
