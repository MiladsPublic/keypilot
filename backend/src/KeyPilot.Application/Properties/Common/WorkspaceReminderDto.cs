using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Common;

public sealed record WorkspaceReminderDto(
    Guid Id,
    Guid? TaskId,
    string Key,
    string Title,
    DateTime ScheduledForUtc,
    string Status,
    DateTime? SentAtUtc,
    DateTime? CancelledAtUtc)
{
    public static WorkspaceReminderDto FromReminder(WorkspaceReminder reminder)
    {
        return new WorkspaceReminderDto(
            reminder.Id,
            reminder.TaskId,
            reminder.Key,
            reminder.Title,
            reminder.ScheduledForUtc,
            EnumText.WorkspaceReminderStatus(reminder.Status),
            reminder.SentAtUtc,
            reminder.CancelledAtUtc);
    }
}