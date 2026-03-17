using Temporalio.Workflows;

namespace KeyPilot.Workflows;

[Workflow]
public sealed class WorkspaceWorkflow
{
    private readonly List<DateTime> _pendingReminderDatesUtc = [];
    private bool _isClosed;

    [WorkflowRun]
    public async Task RunAsync(WorkspaceWorkflowInput input)
    {
        _pendingReminderDatesUtc.Add(input.SettlementReminderAtUtc);
        _pendingReminderDatesUtc.AddRange(input.InitialConditionReminderDatesUtc);

        if (input.BuyingMethod is "auction" or "deadline" && input.MethodSpecificReminderAtUtc.HasValue)
        {
            _pendingReminderDatesUtc.Add(input.MethodSpecificReminderAtUtc.Value);
        }

        while (!_isClosed)
        {
            var nextReminderUtc = GetNextReminderUtc();

            if (nextReminderUtc is null)
            {
                await Workflow.WaitConditionAsync(() => _isClosed || _pendingReminderDatesUtc.Count > 0);
                continue;
            }

            var delay = nextReminderUtc.Value - Workflow.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                await Workflow.DelayAsync(delay);
            }

            await Workflow.ExecuteActivityAsync(
                (IWorkspaceWorkflowActivities activities) => activities.ProcessReminderTickAsync(
                    new WorkspaceReminderTickInput(input.WorkspaceId, Workflow.UtcNow)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(1)
                });

            _pendingReminderDatesUtc.Remove(nextReminderUtc.Value);
        }
    }

    [WorkflowSignal]
    public Task SignalWorkspaceEventAsync(WorkspaceWorkflowSignalInput signal)
    {
        if (signal.EventType is "workspace_cancelled" or "workspace_settled")
        {
            _isClosed = true;
        }

        return Task.CompletedTask;
    }

    [WorkflowSignal]
    public Task SignalReminderScheduleAsync(DateTime reminderAtUtc)
    {
        _pendingReminderDatesUtc.Add(reminderAtUtc);
        return Task.CompletedTask;
    }

    private DateTime? GetNextReminderUtc()
    {
        if (_pendingReminderDatesUtc.Count == 0)
        {
            return null;
        }

        return _pendingReminderDatesUtc.Min();
    }
}
