using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.CancelProperty;

public sealed record CancelPropertyCommand(Guid Id, string OwnerUserId) : IRequest<PropertyDto?>;
