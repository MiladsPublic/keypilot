using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using MediatR;

namespace KeyPilot.Application.Properties.GetPropertyById;

public sealed class GetPropertyByIdHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<GetPropertyByIdQuery, PropertyDto?>
{
    public async Task<PropertyDto?> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow);

        if (property is not null)
        {
            // Keep stage/expiry derivation canonical even for read-only flows.
            property.RecalculateStatus(today);
        }

        return property is null ? null : PropertyDto.FromProperty(property, today);
    }
}
