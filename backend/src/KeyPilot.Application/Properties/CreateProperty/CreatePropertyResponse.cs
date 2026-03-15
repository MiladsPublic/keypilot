using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.CreateProperty;

public sealed record CreatePropertyResponse(
    Guid Id,
    Guid? WorkspaceId,
    string Address,
    string Status,
    DateOnly? OfferAcceptedDate,
    DateOnly? SettlementDate,
    decimal? PurchasePrice,
    DateTime CreatedAtUtc)
{
    public static CreatePropertyResponse FromProperty(Property property)
    {
        return new CreatePropertyResponse(
            property.Id,
            property.WorkspaceId,
            property.Address,
            property.Status.ToString().ToLowerInvariant(),
            property.OfferAcceptedDate,
            property.SettlementDate,
            property.PurchasePrice,
            property.CreatedAtUtc);
    }
}
