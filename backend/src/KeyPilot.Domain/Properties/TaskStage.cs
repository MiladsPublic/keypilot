namespace KeyPilot.Domain.Properties;

public enum TaskStage
{
    Discovery = 0,
    OfferPreparation = 1,
    Submitted = 2,
    Conditional = 3,
    Unconditional = 4,
    SettlementPending = 5,
    Settlement = 6
}
