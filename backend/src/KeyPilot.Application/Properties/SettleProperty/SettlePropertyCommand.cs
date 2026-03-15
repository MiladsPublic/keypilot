using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.SettleProperty;

public sealed record SettlePropertyCommand(Guid Id) : IRequest<PropertyDto?>;
