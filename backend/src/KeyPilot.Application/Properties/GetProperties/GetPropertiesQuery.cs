using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.GetProperties;

public sealed record GetPropertiesQuery(string OwnerUserId) : IRequest<IReadOnlyCollection<PropertyDto>>;
