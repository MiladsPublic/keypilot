namespace KeyPilot.Application.Properties.Common;

public sealed record PurchaseReadinessDto(
    string Mode,
    int BlockingConditions,
    int OpenConditions,
    int OverdueConditions,
    int PendingTasks,
    int OverdueTasks,
    int SettlementTasksRemaining,
    bool IsReadyToSettle,
    string? NextAction);
