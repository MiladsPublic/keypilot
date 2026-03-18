using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.TaskGeneration;

internal sealed class SettlementChecklistGenerator(ITaskTemplateService taskTemplateService) : ISettlementChecklistGenerator
{
    public void EnsureGenerated(Property property, DateTime createdAtUtc)
    {
        if (property.Status != PropertyStatus.Unconditional)
        {
            return;
        }

        var existing = property.Tasks
            .Select(task => $"{task.Stage}:{task.Title}".ToLowerInvariant())
            .ToHashSet();

        foreach (var title in taskTemplateService.GetPreSettlementTasks())
        {
            var key = $"{TaskStage.SettlementPending}:{title}".ToLowerInvariant();
            if (!existing.Contains(key))
            {
                property.AddTask(title, TaskStage.SettlementPending, property.SettlementDate, conditionId: null, createdAtUtc,
                    importance: TaskImportance.Mandatory);
                existing.Add(key);
            }
        }

        foreach (var title in taskTemplateService.GetSettlementTasks())
        {
            var key = $"{TaskStage.Settlement}:{title}".ToLowerInvariant();
            if (!existing.Contains(key))
            {
                property.AddTask(title, TaskStage.Settlement, property.SettlementDate, conditionId: null, createdAtUtc,
                    importance: TaskImportance.Mandatory);
                existing.Add(key);
            }
        }
    }
}
