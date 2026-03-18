using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Common;

public sealed record TaskDto(
    Guid Id,
    Guid? ConditionId,
    string Title,
    string? Description,
    string Importance,
    string? Notes,
    string Stage,
    DateOnly? DueDate,
    string Status,
    DateTime? CompletedAtUtc)
{
    public static TaskDto FromTask(PropertyTask task)
    {
        return new TaskDto(
            task.Id,
            task.ConditionId,
            task.Title,
            task.Description,
            EnumText.TaskImportance(task.Importance),
            task.Notes,
            EnumText.TaskStage(task.Stage),
            task.DueDate,
            EnumText.TaskStatus(task.Status),
            task.CompletedAtUtc);
    }
}
