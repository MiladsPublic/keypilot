namespace KeyPilot.Application.Abstractions.Workflow;

public sealed record WorkspaceWorkflowStartInput(
    Guid WorkspaceId,
    Guid PropertyId,
    string OwnerUserId,
    DateOnly SettlementDate,
    IReadOnlyCollection<DateOnly> PendingConditionDueDates);