namespace KeyPilot.Workflows;

public interface IWorkspaceWorkflowActivities
{
    Task ProcessReminderTickAsync(WorkspaceReminderTickInput input);
}
