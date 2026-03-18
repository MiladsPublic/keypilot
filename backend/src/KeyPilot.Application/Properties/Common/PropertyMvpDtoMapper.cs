using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Common;

internal static class PropertyMvpDtoMapper
{
    public static int? DaysUntilSettlement(Property property, DateOnly today)
    {
        if (!property.SettlementDate.HasValue)
        {
            return null;
        }

        return property.SettlementDate.Value.DayNumber - today.DayNumber;
    }

    public static TaskSummaryDto TaskSummary(Property property)
    {
        var total = property.Tasks.Count;
        var completed = property.Tasks.Count(task => task.Status == Domain.Properties.TaskStatus.Completed);
        return new TaskSummaryDto(completed, total, total - completed);
    }

    public static PurchaseReadinessDto ReadinessSummary(Property property, DateOnly today)
    {
        var openConditions = property.Conditions.Count(condition => condition.Status is ConditionStatus.Pending or ConditionStatus.Failed or ConditionStatus.Expired);
        var overdueConditions = property.Conditions.Count(condition => condition.Status == ConditionStatus.Pending && condition.DueDate < today);
        var blockingConditions = property.Conditions.Count(condition => condition.Status is ConditionStatus.Pending or ConditionStatus.Failed or ConditionStatus.Expired);

        var pendingTasks = property.Tasks.Count(task => task.Status == Domain.Properties.TaskStatus.Pending);
        var overdueTasks = property.Tasks.Count(task => task.Status == Domain.Properties.TaskStatus.Pending && task.DueDate.HasValue && task.DueDate.Value < today);
        var settlementTasksRemaining = property.Tasks.Count(task =>
            task.Status == Domain.Properties.TaskStatus.Pending &&
            (task.Stage == TaskStage.SettlementPending || task.Stage == TaskStage.Settlement));

        var mode = property.Status == PropertyStatus.Unconditional || property.Status == PropertyStatus.SettlementPending
            ? "settlement"
            : "conditional";

        var isReadyToSettle =
            mode == "settlement" &&
            settlementTasksRemaining == 0 &&
            blockingConditions == 0 &&
            !property.SettledDate.HasValue &&
            !property.CancelledDate.HasValue;

        string? nextAction = null;

        var nextOverdueCondition = property.Conditions
            .Where(condition => condition.Status == ConditionStatus.Pending && condition.DueDate < today)
            .OrderBy(condition => condition.DueDate)
            .FirstOrDefault();

        if (nextOverdueCondition is not null)
        {
            nextAction = $"Resolve overdue {EnumText.ConditionType(nextOverdueCondition.Type)} condition";
        }
        else
        {
            var nextPendingTask = property.Tasks
                .Where(task => task.Status == Domain.Properties.TaskStatus.Pending)
                .OrderBy(task => task.DueDate)
                .ThenBy(task => task.Title)
                .FirstOrDefault();

            if (nextPendingTask is not null)
            {
                nextAction = nextPendingTask.Title;
            }
        }

        var (nextCriticalDate, nextCriticalDateLabel) = NextCriticalDeadline(property, today);

        return new PurchaseReadinessDto(
            mode,
            blockingConditions,
            openConditions,
            overdueConditions,
            pendingTasks,
            overdueTasks,
            settlementTasksRemaining,
            isReadyToSettle,
            nextAction,
            nextCriticalDate,
            nextCriticalDateLabel);
    }

    private static (DateOnly? Date, string? Label) NextCriticalDeadline(Property property, DateOnly today)
    {
        if (property.Status is PropertyStatus.Settled or PropertyStatus.Cancelled or PropertyStatus.Archived)
        {
            return (null, null);
        }

        var candidates = new List<(DateOnly Date, string Label)>();

        foreach (var condition in property.Conditions.Where(c => c.Status == ConditionStatus.Pending))
        {
            candidates.Add((condition.DueDate, $"{EnumText.ConditionType(condition.Type)} condition due"));
        }

        if (property.SettlementDate.HasValue)
        {
            candidates.Add((property.SettlementDate.Value, "Settlement"));
        }

        if (property.BuyingMethod == BuyingMethod.Auction && property.AcceptedOfferDate.HasValue &&
            property.Status is PropertyStatus.Discovery or PropertyStatus.OfferPreparation)
        {
            candidates.Add((property.AcceptedOfferDate.Value, "Auction day"));
        }

        var upcoming = candidates
            .Where(c => c.Date >= today)
            .OrderBy(c => c.Date)
            .FirstOrDefault();

        return upcoming == default ? (null, null) : (upcoming.Date, upcoming.Label);
    }
}
