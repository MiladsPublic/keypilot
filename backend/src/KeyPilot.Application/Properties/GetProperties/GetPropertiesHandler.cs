using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.GetProperties;

public sealed class GetPropertiesHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<GetPropertiesQuery, IReadOnlyCollection<PropertyDto>>
{
    public async Task<IReadOnlyCollection<PropertyDto>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        var properties = await dbContext.ListPropertiesByOwnerAsync(request.OwnerUserId, cancellationToken);
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow);

        return properties
            .OrderByDescending(property => property.CreatedAtUtc)
            .Select(property => PropertyDto.FromProperty(property, today))
            .ToArray();
    }
}
