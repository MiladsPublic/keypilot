using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.Common;
using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Reminders;

internal sealed class WorkspaceReminderSyncService(IApplicationDbContext dbContext) : IWorkspaceReminderSyncService
{
    public async Task SyncAsync(Property workspace, DateTime nowUtc, CancellationToken cancellationToken)
    {
        var existing = await dbContext.ListRemindersByPropertyAsync(workspace.Id, cancellationToken);

        var desired = BuildDesiredReminders(workspace, nowUtc).ToArray();
        var desiredByKey = desired.ToDictionary(item => item.Key, item => item);

        foreach (var reminder in existing)
        {
            if (desiredByKey.ContainsKey(reminder.Key))
            {
                continue;
            }

            if (reminder.Status == WorkspaceReminderStatus.Pending)
            {
                reminder.MarkCancelled(nowUtc);
            }
        }

        var existingKeys = existing.Select(reminder => reminder.Key).ToHashSet();

        foreach (var reminder in desired)
        {
            if (existingKeys.Contains(reminder.Key))
            {
                continue;
            }

            workspace.AddReminder(reminder.Key, reminder.Title, reminder.ScheduledForUtc, nowUtc);
        }

        // TODO: Temporal workflow activities should trigger dispatch for pending reminders;
        // this service only keeps persisted reminder intent as the source of truth.
    }

    private static IEnumerable<(string Key, string Title, DateTime ScheduledForUtc)> BuildDesiredReminders(Property workspace, DateTime nowUtc)
    {
        if (workspace.Status is PropertyStatus.Settled or PropertyStatus.Cancelled)
        {
            yield break;
        }

        foreach (var condition in workspace.Conditions.Where(condition => condition.Status == ConditionStatus.Pending))
        {
            var label = EnumText.ConditionType(condition.Type);

            foreach (var (suffix, daysBefore, titlePrefix) in EscalationOffsets)
            {
                yield return (
                    $"condition:{condition.Id}:{suffix}",
                    $"{titlePrefix}{label} condition",
                    ToUtcAtNineAm(condition.DueDate.AddDays(-daysBefore), nowUtc));
            }
        }

        if (workspace.BuyingMethod == BuyingMethod.Auction && workspace.AcceptedOfferDate.HasValue)
        {
            yield return (
                "auction:readiness",
                "Auction readiness check",
                ToUtcAtNineAm(workspace.AcceptedOfferDate.Value.AddDays(-1), nowUtc));
        }

        if (workspace.BuyingMethod == BuyingMethod.Deadline && workspace.SettlementDate.HasValue)
        {
            yield return (
                "deadline:readiness",
                "Deadline readiness check",
                ToUtcAtNineAm(workspace.SettlementDate.Value.AddDays(-1), nowUtc));
        }

        if (workspace.SettlementDate.HasValue)
        {
            foreach (var (suffix, daysBefore, titlePrefix) in EscalationOffsets)
            {
                yield return (
                    $"settlement:{suffix}",
                    $"{titlePrefix}settlement",
                    ToUtcAtNineAm(workspace.SettlementDate.Value.AddDays(-daysBefore), nowUtc));
            }
        }
    }

    private static readonly (string Suffix, int DaysBefore, string TitlePrefix)[] EscalationOffsets =
    [
        ("7d", 7, "7 days until "),
        ("3d", 3, "3 days until "),
        ("1d", 1, "Tomorrow: "),
        ("due", 0, "Due today: ")
    ];

    private static DateTime ToUtcAtNineAm(DateOnly date, DateTime nowUtc)
    {
        var scheduled = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(9);
        return scheduled < nowUtc ? nowUtc : scheduled;
    }
}