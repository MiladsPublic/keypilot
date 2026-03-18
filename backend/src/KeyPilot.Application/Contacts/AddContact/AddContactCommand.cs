using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Contacts.AddContact;

public sealed record AddContactCommand(
    Guid PropertyId,
    string Role,
    string Name,
    string? Email,
    string? Phone,
    string OwnerUserId) : IRequest<ContactDto?>;
