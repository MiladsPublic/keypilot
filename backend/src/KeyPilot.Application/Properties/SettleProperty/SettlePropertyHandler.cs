using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.SettleProperty;

public sealed class SettlePropertyHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<SettlePropertyCommand, PropertyDto?>
{
    public async Task<PropertyDto?> Handle(SettlePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        var now = dateTimeProvider.UtcNow;
        property.MarkSettlementComplete(now);
        await dbContext.SaveChangesAsync(cancellationToken);

        return PropertyDto.FromProperty(property, DateOnly.FromDateTime(now));
    }
}
