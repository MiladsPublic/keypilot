using KeyPilot.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyPilot.Infrastructure.Persistence.Configurations;

public sealed class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("properties");

        builder.HasKey(property => property.Id);

        builder.Property(property => property.Id)
            .HasColumnName("id");

        builder.Property(property => property.WorkspaceId)
            .HasColumnName("workspace_id");

        builder.Property(property => property.Address)
            .HasColumnName("address")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(property => property.Status)
            .HasColumnName("status")
            .HasConversion(
                value => value.ToString().ToLowerInvariant(),
                value => Enum.Parse<PropertyStatus>(value, ignoreCase: true))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(property => property.OfferAcceptedDate)
            .HasColumnName("offer_accepted_date");

        builder.Property(property => property.SettlementDate)
            .HasColumnName("settlement_date");

        builder.Property(property => property.PurchasePrice)
            .HasColumnName("purchase_price")
            .HasColumnType("numeric(12,2)");

        builder.Property(property => property.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();
    }
}
