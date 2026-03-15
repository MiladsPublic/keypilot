using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.GetProperties;

public sealed class GetPropertiesHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetPropertiesQuery, IReadOnlyCollection<PropertyDto>>
{
    public async Task<IReadOnlyCollection<PropertyDto>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        var properties = await dbContext.ListPropertiesByOwnerAsync(request.OwnerUserId, cancellationToken);

        return properties
            .OrderByDescending(property => property.CreatedAtUtc)
            .Select(PropertyDto.FromProperty)
            .ToArray();
    }
}
