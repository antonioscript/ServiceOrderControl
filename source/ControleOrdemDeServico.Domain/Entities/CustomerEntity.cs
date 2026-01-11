using OsService.Domain.Common;

namespace OsService.Domain.Entities
{
    public class CustomerEntity : BaseEntity
    {
        public string Name { get; init; } = default!;
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public string? Document { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.Now;

    }
}
