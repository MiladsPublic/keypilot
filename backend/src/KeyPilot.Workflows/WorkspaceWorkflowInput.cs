namespace KeyPilot.Workflows;

public sealed record WorkspaceWorkflowInput(
    Guid WorkspaceId,
    Guid PropertyId,
    string OwnerUserId,
    string BuyingMethod,
    DateTime? MethodSpecificReminderAtUtc,
    DateTime SettlementReminderAtUtc,
    IReadOnlyCollection<DateTime> InitialConditionReminderDatesUtc);
