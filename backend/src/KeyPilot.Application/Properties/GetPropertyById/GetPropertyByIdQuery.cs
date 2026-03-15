using MediatR;

namespace KeyPilot.Application.Properties.GetPropertyById;

public sealed record GetPropertyByIdQuery(Guid Id) : IRequest<PropertyDto?>;
