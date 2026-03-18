using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.GoUnconditional;

public sealed record GoUnconditionalCommand(Guid Id, string OwnerUserId) : IRequest<PropertyDto?>;
