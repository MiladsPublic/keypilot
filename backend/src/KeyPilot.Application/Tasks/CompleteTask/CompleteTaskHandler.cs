using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Application.Properties.Common;
using KeyPilot.Application.Properties.Lifecycle;
using KeyPilot.Application.Properties.Reminders;
using MediatR;

namespace KeyPilot.Application.Tasks.CompleteTask;

public sealed class CompleteTaskHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IWorkspaceWorkflowOrchestrator workflowOrchestrator,
    IWorkspaceLifecycleService workspaceLifecycleService,
    IWorkspaceReminderSyncService workspaceReminderSyncService) : IRequestHandler<CompleteTaskCommand, TaskDto?>
{
    public async Task<TaskDto?> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await dbContext.GetTaskByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (task is null)
        {
            return null;
        }

        task.MarkCompleted(dateTimeProvider.UtcNow);

        var property = await dbContext.GetPropertyByIdAsync(task.PropertyId, request.OwnerUserId, cancellationToken);

        if (property is not null)
        {
            workspaceLifecycleService.ApplyDerivedState(property, DateOnly.FromDateTime(dateTimeProvider.UtcNow));
            await workspaceReminderSyncService.SyncAsync(property, dateTimeProvider.UtcNow, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        if (property?.WorkspaceId is Guid workspaceId)
        {
            await workflowOrchestrator.SignalAsync(
                new WorkspaceWorkflowSignal(
                    workspaceId,
                    EventType: "task_completed",
                    OccurredAtUtc: dateTimeProvider.UtcNow,
                    TaskId: task.Id),
                cancellationToken);
        }

        return TaskDto.FromTask(task);
    }
}
