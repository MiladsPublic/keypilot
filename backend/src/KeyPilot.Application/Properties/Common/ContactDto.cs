using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Common;

public sealed record ContactDto(
    Guid Id,
    string Role,
    string Name,
    string? Email,
    string? Phone,
    DateTime CreatedAtUtc)
{
    public static ContactDto FromContact(Contact contact)
    {
        return new ContactDto(
            contact.Id,
            contact.Role,
            contact.Name,
            contact.Email,
            contact.Phone,
            contact.CreatedAtUtc);
    }
}
