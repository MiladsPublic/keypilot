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
    IWorkspaceWorkflowOutbox workflowOutbox,
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

        if (property?.WorkspaceId is Guid workspaceId)
        {
            await workflowOutbox.EnqueueWorkspaceSignalAsync(
                    workspaceId,
                    eventType: "task_completed",
                    occurredAtUtc: dateTimeProvider.UtcNow,
                    cancellationToken: cancellationToken,
                    taskId: task.Id);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return TaskDto.FromTask(task);
    }
}
