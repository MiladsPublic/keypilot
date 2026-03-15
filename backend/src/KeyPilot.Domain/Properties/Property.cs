using KeyPilot.Domain.Common;

namespace KeyPilot.Domain.Properties;

public sealed class Property : AuditableEntity
{
    private Property()
    {
    }

    private Property(
        string address,
        DateOnly? offerAcceptedDate,
        DateOnly? settlementDate,
        decimal? purchasePrice,
        Guid? workspaceId,
        DateTime createdAtUtc)
    {
        Address = address;
        OfferAcceptedDate = offerAcceptedDate;
        SettlementDate = settlementDate;
        PurchasePrice = purchasePrice;
        WorkspaceId = workspaceId;
        Status = PropertyStatus.Conditional;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid? WorkspaceId { get; private set; }

    public string Address { get; private set; } = string.Empty;

    public PropertyStatus Status { get; private set; } = PropertyStatus.Draft;

    public DateOnly? OfferAcceptedDate { get; private set; }

    public DateOnly? SettlementDate { get; private set; }

    public decimal? PurchasePrice { get; private set; }

    public static Property Create(
        string address,
        DateOnly? offerAcceptedDate,
        DateOnly? settlementDate,
        decimal? purchasePrice,
        Guid? workspaceId,
        DateTime createdAtUtc)
    {
        return new Property(address, offerAcceptedDate, settlementDate, purchasePrice, workspaceId, createdAtUtc);
    }
}
