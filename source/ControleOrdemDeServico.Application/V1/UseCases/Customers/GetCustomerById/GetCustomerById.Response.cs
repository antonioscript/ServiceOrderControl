namespace OsService.Application.V1.UseCases.Customers.GetCustomerById;

public partial class GetCustomerById 
{
    public sealed record Response(
    Guid Id,
    string Name,
    string? Phone,
    string? Email,
    string? Document,
    DateTime CreatedAt);

}

