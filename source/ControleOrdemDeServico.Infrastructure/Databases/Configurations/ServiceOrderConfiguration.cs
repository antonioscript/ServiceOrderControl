using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsService.Domain.Entities;

namespace OsService.Infrastructure.Databases.Configurations;

public class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrderEntity>
{
    public void Configure(EntityTypeBuilder<ServiceOrderEntity> builder)
    {
        builder.ToTable("ServiceOrders", "dbo");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        //builder.Property(x => x.Number)
        //    .ValueGeneratedOnAdd(); //TODO

        builder.Property(x => x.Number)
            .UseIdentityColumn(seed: 1000, increment: 1)
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.OpenedAt)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Coin)
            .HasMaxLength(4);

        builder.Property(x => x.UpdatedPriceAt);

        builder.HasIndex(x => x.CustomerId);
    }
}