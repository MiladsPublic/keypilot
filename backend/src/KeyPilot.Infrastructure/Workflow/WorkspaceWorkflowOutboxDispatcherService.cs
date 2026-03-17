using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Domain.Workflows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KeyPilot.Infrastructure.Workflow;

public sealed class WorkspaceWorkflowOutboxDispatcherService(
    IServiceScopeFactory scopeFactory,
    ILogger<WorkspaceWorkflowOutboxDispatcherService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var orchestrator = scope.ServiceProvider.GetRequiredService<IWorkspaceWorkflowOrchestrator>();
                var clock = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

                var pending = await dbContext.ListPendingWorkflowEventsAsync(25, stoppingToken);

                foreach (var workflowEvent in pending)
                {
                    try
                    {
                        await DispatchEventAsync(dbContext, orchestrator, clock.UtcNow, workflowEvent, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        workflowEvent.MarkAttemptFailed(ex.Message);
                        logger.LogWarning(ex, "Failed processing workflow outbox event {EventId}", workflowEvent.Id);
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Workflow outbox dispatcher cycle failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

    private static async Task DispatchEventAsync(
        IApplicationDbContext dbContext,
        IWorkspaceWorkflowOrchestrator orchestrator,
        DateTime nowUtc,
        WorkspaceWorkflowEvent workflowEvent,
        CancellationToken cancellationToken)
    {
        if (workflowEvent.EventType == "workspace_created")
        {
            var workspace = await dbContext.GetPropertyByWorkspaceIdAsync(workflowEvent.WorkspaceId, cancellationToken);

            if (workspace is null || !workspace.WorkspaceId.HasValue)
            {
                workflowEvent.MarkAttemptFailed("Workspace not found while dispatching start event.");
                return;
            }

            var pendingConditionDueDates = workspace.Conditions
                .Where(condition => condition.Status == Domain.Properties.ConditionStatus.Pending)
                .Select(condition => condition.DueDate)
                .ToArray();

            await orchestrator.StartAsync(
                new WorkspaceWorkflowStartInput(
                    workspace.WorkspaceId.Value,
                    workspace.Id,
                    workspace.OwnerUserId,
                    workspace.BuyingMethod,
                    workspace.AcceptedOfferDate,
                    workspace.SettlementDate,
                    pendingConditionDueDates),
                cancellationToken);

            var instance = await dbContext.GetWorkflowInstanceByWorkspaceIdAsync(workflowEvent.WorkspaceId, cancellationToken);
            var workflowId = $"workspace-{workflowEvent.WorkspaceId}";

            if (instance is null)
            {
                await dbContext.AddWorkflowInstanceAsync(
                    WorkspaceWorkflowInstance.Start(
                        workflowEvent.WorkspaceId,
                        workflowId,
                        startedAtUtc: nowUtc,
                        createdAtUtc: nowUtc),
                    cancellationToken);
            }
            else
            {
                instance.MarkSignaled(nowUtc);
            }
        }
        else
        {
            await orchestrator.SignalAsync(
                new WorkspaceWorkflowSignal(
                    workflowEvent.WorkspaceId,
                    workflowEvent.EventType,
                    workflowEvent.OccurredAtUtc,
                    workflowEvent.TaskId,
                    workflowEvent.ConditionId),
                cancellationToken);

            var instance = await dbContext.GetWorkflowInstanceByWorkspaceIdAsync(workflowEvent.WorkspaceId, cancellationToken);
            if (instance is not null)
            {
                instance.MarkSignaled(nowUtc);

                if (workflowEvent.EventType is "workspace_settled" or "workspace_cancelled")
                {
                    instance.MarkClosed(nowUtc);
                }
            }
        }

        workflowEvent.MarkProcessed(nowUtc);
    }
}
