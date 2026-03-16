using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.CancelProperty;

public sealed class CancelPropertyHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CancelPropertyCommand, PropertyDto?>
{
    public async Task<PropertyDto?> Handle(CancelPropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        var now = dateTimeProvider.UtcNow;
        property.MarkCancelled(now);

        await dbContext.SaveChangesAsync(cancellationToken);

        return PropertyDto.FromProperty(property, DateOnly.FromDateTime(now));
    }
}
