using MediatR;

namespace KeyPilot.Application.Contacts.DeleteContact;

public sealed record DeleteContactCommand(Guid Id, string OwnerUserId) : IRequest<bool>;
