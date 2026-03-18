using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.ArchiveProperty;

public sealed record ArchivePropertyCommand(Guid Id, string OwnerUserId) : IRequest<PropertyDto?>;
