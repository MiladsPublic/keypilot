using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.GetPropertyById;

public sealed record PropertyDto(
    Guid Id,
    Guid? WorkspaceId,
    string Address,
    string Status,
    DateOnly? OfferAcceptedDate,
    DateOnly? SettlementDate,
    decimal? PurchasePrice,
    DateTime CreatedAtUtc)
{
    public static PropertyDto FromProperty(Property property)
    {
        return new PropertyDto(
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
