using KeyPilot.Domain.Common;

namespace KeyPilot.Domain.Properties;

public sealed class Contact : AuditableEntity
{
    private Contact()
    {
    }

    internal Contact(
        Guid propertyId,
        string role,
        string name,
        string? email,
        string? phone,
        DateTime createdAtUtc)
    {
        PropertyId = propertyId;
        Role = role;
        Name = name;
        Email = email;
        Phone = phone;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid PropertyId { get; private set; }

    public string Role { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public string? Phone { get; private set; }
}
