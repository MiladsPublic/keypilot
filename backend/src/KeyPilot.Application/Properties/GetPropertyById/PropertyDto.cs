using KeyPilot.Application.Properties.Common;
using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.GetPropertyById;

public sealed record PropertyDto(
    Guid Id,
    Guid? WorkspaceId,
    string Address,
    string BuyingMethod,
    string WorkspaceStage,
    string Status,
    DateOnly AcceptedOfferDate,
    DateOnly? UnconditionalDate,
    DateOnly SettlementDate,
    DateOnly? SettledDate,
    DateOnly? CancelledDate,
    int DaysUntilSettlement,
    decimal? PurchasePrice,
    decimal? DepositAmount,
    IReadOnlyCollection<WorkspaceReminderDto> Reminders,
    IReadOnlyCollection<ConditionDto> Conditions,
    IReadOnlyCollection<TaskDto> Tasks,
    TaskSummaryDto TaskSummary,
    PurchaseReadinessDto ReadinessSummary,
    DateTime CreatedAtUtc)
{
    public static PropertyDto FromProperty(Property property, DateOnly today)
    {
        return new PropertyDto(
            property.Id,
            property.WorkspaceId,
            property.Address,
            EnumText.BuyingMethod(property.BuyingMethod),
            EnumText.WorkspaceStage(property.Status),
            EnumText.PropertyStatus(property.Status),
            property.AcceptedOfferDate,
            property.UnconditionalDate,
            property.SettlementDate,
            property.SettledDate,
            property.CancelledDate,
            PropertyMvpDtoMapper.DaysUntilSettlement(property, today),
            property.PurchasePrice,
            property.DepositAmount,
            property.Reminders
                .OrderBy(reminder => reminder.ScheduledForUtc)
                .Select(WorkspaceReminderDto.FromReminder)
                .ToArray(),
            property.Conditions
                .OrderBy(condition => condition.DueDate)
                .Select(ConditionDto.FromCondition)
                .ToArray(),
            property.Tasks
                .OrderBy(task => task.Stage)
                .ThenBy(task => task.DueDate)
                .ThenBy(task => task.Title)
                .Select(TaskDto.FromTask)
                .ToArray(),
            PropertyMvpDtoMapper.TaskSummary(property),
            PropertyMvpDtoMapper.ReadinessSummary(property, today),
            property.CreatedAtUtc);
    }
}
