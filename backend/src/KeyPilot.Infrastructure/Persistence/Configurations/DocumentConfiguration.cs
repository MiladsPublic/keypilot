using KeyPilot.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyPilot.Infrastructure.Persistence.Configurations;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");

        builder.HasKey(document => document.Id);

        builder.Property(document => document.Id)
            .HasColumnName("id");

        builder.Property(document => document.PropertyId)
            .HasColumnName("property_id")
            .IsRequired();

        builder.Property(document => document.StorageKey)
            .HasColumnName("storage_key")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(document => document.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(document => document.Category)
            .HasColumnName("category")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(document => document.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(document => document.PropertyId);
    }
}
