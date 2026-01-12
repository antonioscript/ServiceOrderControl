namespace OsService.Application.V1.UseCases.Customers.GetCustomerById;

public sealed record GetCustomerByIdResponse(
    Guid Id,
    string Name,
    string? Phone,
    string? Email,
    string? Document,
    DateTime CreatedAt);