using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Common;

internal static class PropertyMvpDtoMapper
{
    public static int DaysUntilSettlement(Property property)
    {
        var diff = property.SettlementDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
        return diff;
    }

    public static TaskSummaryDto TaskSummary(Property property)
    {
        var total = property.Tasks.Count;
        var completed = property.Tasks.Count(task => task.Status == Domain.Properties.TaskStatus.Completed);
        return new TaskSummaryDto(completed, total, total - completed);
    }
}
