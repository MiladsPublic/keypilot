using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Abstractions.Workflow;

public sealed record WorkspaceWorkflowStartInput(
    Guid WorkspaceId,
    Guid PropertyId,
    string OwnerUserId,
    BuyingMethod BuyingMethod,
    DateOnly AcceptedOfferDate,
    DateOnly SettlementDate,
    IReadOnlyCollection<DateOnly> PendingConditionDueDates);