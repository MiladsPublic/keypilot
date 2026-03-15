using KeyPilot.Domain.Common;

namespace KeyPilot.Domain.Properties;

public sealed class Condition : AuditableEntity
{
    private Condition()
    {
    }

    internal Condition(
        Guid propertyId,
        ConditionType type,
        DateOnly dueDate,
        DateTime createdAtUtc)
    {
        PropertyId = propertyId;
        Type = type;
        DueDate = dueDate;
        Status = ConditionStatus.Pending;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid PropertyId { get; private set; }

    public ConditionType Type { get; private set; }

    public DateOnly DueDate { get; private set; }

    public ConditionStatus Status { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public void MarkCompleted(DateTime completedAtUtc)
    {
        if (Status == ConditionStatus.Completed)
        {
            return;
        }

        Status = ConditionStatus.Completed;
        CompletedAtUtc = completedAtUtc;
    }
}
