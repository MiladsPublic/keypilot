namespace KeyPilot.Domain.Properties;

public enum PropertyStatus
{
    Discovery = 0,
    OfferPreparation = 1,
    Submitted = 2,
    Conditional = 3,
    Unconditional = 4,
    SettlementPending = 5,
    Settled = 6,
    Archived = 7,
    Cancelled = 8
}
