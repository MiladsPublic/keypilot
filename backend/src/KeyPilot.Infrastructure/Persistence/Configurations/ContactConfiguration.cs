using KeyPilot.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyPilot.Infrastructure.Persistence.Configurations;

public sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("contacts");

        builder.HasKey(contact => contact.Id);

        builder.Property(contact => contact.Id)
            .HasColumnName("id");

        builder.Property(contact => contact.PropertyId)
            .HasColumnName("property_id")
            .IsRequired();

        builder.Property(contact => contact.Role)
            .HasColumnName("role")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(contact => contact.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(contact => contact.Email)
            .HasColumnName("email")
            .HasMaxLength(250);

        builder.Property(contact => contact.Phone)
            .HasColumnName("phone")
            .HasMaxLength(30);

        builder.Property(contact => contact.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(contact => contact.PropertyId);
    }
}
