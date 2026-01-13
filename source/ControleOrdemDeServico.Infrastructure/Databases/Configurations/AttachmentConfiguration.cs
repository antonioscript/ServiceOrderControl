using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsService.Domain.Entities;

namespace OsService.Infrastructure.Databases.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<AttachmentEntity>
{
    public void Configure(EntityTypeBuilder<AttachmentEntity> builder)
    {
        builder.ToTable("ServiceOrderAttachments", "dbo");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceOrderId)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SizeBytes)
            .IsRequired();

        builder.Property(x => x.StoragePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.UploadedAt)
            .IsRequired();

        builder.HasIndex(x => x.ServiceOrderId);
    }
}