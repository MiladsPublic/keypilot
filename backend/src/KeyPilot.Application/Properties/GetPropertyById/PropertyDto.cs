using KeyPilot.Application.Properties.Common;
using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.GetPropertyById;

public sealed record PropertyDto(
    Guid Id,
    Guid? WorkspaceId,
    string Address,
    string Status,
    DateOnly AcceptedOfferDate,
    DateOnly? UnconditionalDate,
    DateOnly SettlementDate,
    DateOnly? SettledDate,
    DateOnly? CancelledDate,
    int DaysUntilSettlement,
    decimal? PurchasePrice,
    decimal? DepositAmount,
    IReadOnlyCollection<ConditionDto> Conditions,
    IReadOnlyCollection<TaskDto> Tasks,
    TaskSummaryDto TaskSummary,
    DateTime CreatedAtUtc)
{
    public static PropertyDto FromProperty(Property property)
    {
        return new PropertyDto(
            property.Id,
            property.WorkspaceId,
            property.Address,
            EnumText.PropertyStatus(property.Status),
            property.AcceptedOfferDate,
            property.UnconditionalDate,
            property.SettlementDate,
            property.SettledDate,
            property.CancelledDate,
            PropertyMvpDtoMapper.DaysUntilSettlement(property),
            property.PurchasePrice,
            property.DepositAmount,
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
            property.CreatedAtUtc);
    }
}
