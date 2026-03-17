namespace KeyPilot.Application.Abstractions.Workflow;

public sealed record WorkspaceWorkflowSignal(
    Guid WorkspaceId,
    string EventType,
    DateTime OccurredAtUtc,
    Guid? TaskId = null,
    Guid? ConditionId = null);