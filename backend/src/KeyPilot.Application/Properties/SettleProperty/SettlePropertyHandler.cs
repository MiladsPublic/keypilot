using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.SettleProperty;

public sealed class SettlePropertyHandler(IApplicationDbContext dbContext)
    : IRequestHandler<SettlePropertyCommand, PropertyDto?>
{
    public async Task<PropertyDto?> Handle(SettlePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        property.MarkSettlementComplete();
        await dbContext.SaveChangesAsync(cancellationToken);

        return PropertyDto.FromProperty(property);
    }
}
