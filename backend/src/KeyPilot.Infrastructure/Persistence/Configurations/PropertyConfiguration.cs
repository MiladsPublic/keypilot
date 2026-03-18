using System.Text.RegularExpressions;
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

        builder.Property(property => property.OwnerUserId)
            .HasColumnName("owner_user_id")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(property => property.Status)
            .HasColumnName("status")
            .HasConversion(
                value => ToSnakeCase(value.ToString()),
                value => Enum.Parse<PropertyStatus>(value.Replace("_", ""), ignoreCase: true))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(property => property.BuyingMethod)
            .HasColumnName("buying_method")
            .HasConversion(
                value => ToSnakeCase(value.ToString()),
                value => Enum.Parse<BuyingMethod>(value.Replace("_", ""), ignoreCase: true))
            .HasMaxLength(32)
            .HasDefaultValueSql("'private_sale'")
            .IsRequired();

        builder.Property(property => property.MethodReference)
            .HasColumnName("method_reference")
            .HasMaxLength(100);

        builder.Property(property => property.AcceptedOfferDate)
            .HasColumnName("accepted_offer_date")
            .IsRequired();

        builder.Property(property => property.SettlementDate)
            .HasColumnName("settlement_date")
            .IsRequired();

        builder.Property(property => property.UnconditionalDate)
            .HasColumnName("unconditional_date");

        builder.Property(property => property.SettledDate)
            .HasColumnName("settled_date");

        builder.Property(property => property.CancelledDate)
            .HasColumnName("cancelled_date");

        builder.Property(property => property.PurchasePrice)
            .HasColumnName("purchase_price")
            .HasColumnType("numeric(12,2)");

        builder.Property(property => property.DepositAmount)
            .HasColumnName("deposit_amount")
            .HasColumnType("numeric(12,2)");

        builder.Property(property => property.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasMany(property => property.Conditions)
            .WithOne()
            .HasForeignKey(condition => condition.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(property => property.Reminders)
            .WithOne()
            .HasForeignKey(reminder => reminder.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(property => property.Tasks)
            .WithOne()
            .HasForeignKey(task => task.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(property => property.Documents)
            .WithOne()
            .HasForeignKey(document => document.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(property => property.Contacts)
            .WithOne()
            .HasForeignKey(contact => contact.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(property => property.OwnerUserId);
    }

    private static string ToSnakeCase(string input)
        => Regex.Replace(input, "(?<!^)([A-Z])", "_$1").ToLowerInvariant();
}
