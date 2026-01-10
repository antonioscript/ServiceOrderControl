using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsService.Domain.Entities;

namespace OsService.Infrastructure.Databases.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<CustomerEntity>
{
    public void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        builder.ToTable("Customers", "dbo");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .IsRequired();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Phone)
            .HasMaxLength(30);

        builder.Property(c => c.Email)
            .HasMaxLength(120);

        builder.Property(c => c.Document)
            .HasMaxLength(30);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasIndex(c => c.Phone);
        builder.HasIndex(c => c.Document);
    }
}