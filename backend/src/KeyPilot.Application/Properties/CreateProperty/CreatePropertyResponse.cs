using KeyPilot.Application.Properties.Common;
using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.CreateProperty;

public sealed record CreatePropertyResponse(
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
    IReadOnlyCollection<DocumentDto> Documents,
    IReadOnlyCollection<ContactDto> Contacts,
    TaskSummaryDto TaskSummary,
    PurchaseReadinessDto ReadinessSummary,
    DateTime CreatedAtUtc)
{
    public static CreatePropertyResponse FromProperty(Property property, DateOnly today)
    {
        return new CreatePropertyResponse(
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
            property.Documents
                .OrderBy(document => document.CreatedAtUtc)
                .Select(DocumentDto.FromDocument)
                .ToArray(),
            property.Contacts
                .OrderBy(contact => contact.Name)
                .Select(ContactDto.FromContact)
                .ToArray(),
            PropertyMvpDtoMapper.TaskSummary(property),
            PropertyMvpDtoMapper.ReadinessSummary(property, today),
            property.CreatedAtUtc);
    }
}
