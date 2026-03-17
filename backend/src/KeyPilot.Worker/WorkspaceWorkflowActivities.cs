using KeyPilot.Application.Properties.Reminders.ProcessWorkspaceReminderTick;
using KeyPilot.Workflows;
using MediatR;
using Temporalio.Activities;

namespace KeyPilot.Worker;

public sealed class WorkspaceWorkflowActivities(IServiceScopeFactory scopeFactory) : IWorkspaceWorkflowActivities
{
    [Activity]
    public async Task ProcessReminderTickAsync(WorkspaceReminderTickInput input)
    {
        using var scope = scopeFactory.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(new ProcessWorkspaceReminderTickCommand(input.WorkspaceId, input.TriggeredAtUtc));
    }
}
