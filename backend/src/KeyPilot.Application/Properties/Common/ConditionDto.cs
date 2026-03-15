using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Common;

public sealed record ConditionDto(
    Guid Id,
    string Type,
    DateOnly DueDate,
    string Status,
    DateTime? CompletedAtUtc)
{
    public static ConditionDto FromCondition(Condition condition)
    {
        return new ConditionDto(
            condition.Id,
            EnumText.ConditionType(condition.Type),
            condition.DueDate,
            EnumText.ConditionStatus(condition.Status),
            condition.CompletedAtUtc);
    }
}
