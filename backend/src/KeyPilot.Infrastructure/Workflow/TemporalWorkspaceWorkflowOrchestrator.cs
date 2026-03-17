using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Workflows;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Temporalio.Client;

namespace KeyPilot.Infrastructure.Workflow;

public sealed class TemporalWorkspaceWorkflowOrchestrator(
    ITemporalClientProvider clientProvider,
    IOptions<TemporalOptions> options,
    ILogger<TemporalWorkspaceWorkflowOrchestrator> logger) : IWorkspaceWorkflowOrchestrator
{
    private readonly TemporalOptions _options = options.Value;

    public async Task StartAsync(WorkspaceWorkflowStartInput input, CancellationToken cancellationToken)
    {
        var client = await clientProvider.GetClientAsync(cancellationToken);

        if (client is null)
        {
            return;
        }

        var workflowId = WorkflowId(input.WorkspaceId);
        var reminders = input.PendingConditionDueDates
            .Select(date => date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(9))
            .ToArray();

        try
        {
            await client.StartWorkflowAsync(
                (WorkspaceWorkflow workflow) => workflow.RunAsync(
                    new WorkspaceWorkflowInput(
                        input.WorkspaceId,
                        input.PropertyId,
                        input.OwnerUserId,
                        input.SettlementDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(9),
                        reminders)),
                new WorkflowOptions(
                    id: workflowId,
                    taskQueue: _options.TaskQueue));
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("already", StringComparison.OrdinalIgnoreCase) &&
                ex.Message.Contains("start", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogDebug("Workspace workflow already started for {WorkspaceId}", input.WorkspaceId);
                return;
            }

            throw;
        }
    }

    public async Task SignalAsync(WorkspaceWorkflowSignal signal, CancellationToken cancellationToken)
    {
        var client = await clientProvider.GetClientAsync(cancellationToken);

        if (client is null)
        {
            return;
        }

        var handle = client.GetWorkflowHandle<WorkspaceWorkflow>(WorkflowId(signal.WorkspaceId));

        await handle.SignalAsync(
            workflow => workflow.SignalWorkspaceEventAsync(
                new WorkspaceWorkflowSignalInput(
                    signal.EventType,
                    signal.OccurredAtUtc,
                    signal.TaskId,
                    signal.ConditionId)));

        if (signal.EventType is "condition_satisfied" or "condition_waived" or "condition_failed")
        {
            await handle.SignalAsync(workflow => workflow.SignalReminderScheduleAsync(signal.OccurredAtUtc.AddMinutes(1)));
        }
    }

    private static string WorkflowId(Guid workspaceId)
    {
        return $"workspace-{workspaceId}";
    }
}
