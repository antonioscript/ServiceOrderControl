namespace OsService.Application.V1.Features.Customers.GetCustomerById;

public sealed record GetCustomerByIdResponse(
    Guid Id,
    string Name,
    string? Phone,
    string? Email,
    string? Document,
    DateTime CreatedAt);