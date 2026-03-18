using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.TaskGeneration;

internal sealed class SettlementChecklistGenerator(ITaskTemplateService taskTemplateService) : ISettlementChecklistGenerator
{
    public void EnsureGenerated(Property property, DateTime createdAtUtc)
    {
        if (property.Status is not (PropertyStatus.Unconditional or PropertyStatus.SettlementPending))
        {
            return;
        }

        var existing = property.Tasks
            .Select(task => $"{task.Stage}:{task.Title}".ToLowerInvariant())
            .ToHashSet();

        foreach (var template in taskTemplateService.GetPreSettlementTasks())
        {
            var key = $"{TaskStage.SettlementPending}:{template.Title}".ToLowerInvariant();
            if (!existing.Contains(key))
            {
                property.AddTask(template.Title, TaskStage.SettlementPending, property.SettlementDate, conditionId: null, createdAtUtc,
                    description: template.Description, importance: TaskImportance.Mandatory);
                existing.Add(key);
            }
        }

        foreach (var template in taskTemplateService.GetSettlementTasks())
        {
            var key = $"{TaskStage.Settlement}:{template.Title}".ToLowerInvariant();
            if (!existing.Contains(key))
            {
                property.AddTask(template.Title, TaskStage.Settlement, property.SettlementDate, conditionId: null, createdAtUtc,
                    description: template.Description, importance: TaskImportance.Mandatory);
                existing.Add(key);
            }
        }
    }
}
