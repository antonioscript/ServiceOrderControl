namespace OsService.Application.V1.UseCases.Customers.GetCustomerByContact;

public partial class GetCustomerByContact 
{
    public sealed record Response(
    Guid Id,
    string Name,
    string? Phone,
    string? Email,
    string? Document,
    DateTime CreatedAt);
}

