namespace KeyPilot.Workflows;

public sealed record WorkspaceWorkflowInput(
    Guid WorkspaceId,
    Guid PropertyId,
    string OwnerUserId,
    DateTime SettlementReminderAtUtc,
    IReadOnlyCollection<DateTime> InitialConditionReminderDatesUtc);
