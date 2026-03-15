using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Domain.Properties;
using MediatR;

namespace KeyPilot.Application.Properties.CreateProperty;

public sealed class CreatePropertyHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<CreatePropertyCommand, CreatePropertyResponse>
{
    public async Task<CreatePropertyResponse> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = Property.Create(
            request.Address.Trim(),
            request.OfferAcceptedDate,
            request.SettlementDate,
            request.PurchasePrice,
            workspaceId: null,
            createdAtUtc: dateTimeProvider.UtcNow);

        await dbContext.AddPropertyAsync(property, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatePropertyResponse.FromProperty(property);
    }
}
