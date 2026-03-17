namespace KeyPilot.Workflows;

public sealed record WorkspaceReminderTickInput(
    Guid WorkspaceId,
    DateTime TriggeredAtUtc);
