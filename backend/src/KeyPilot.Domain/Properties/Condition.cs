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

    public void MarkSatisfied(DateTime completedAtUtc)
    {
        if (Status == ConditionStatus.Satisfied)
        {
            return;
        }

        Status = ConditionStatus.Satisfied;
        CompletedAtUtc = completedAtUtc;
    }

    public void MarkWaived(DateTime completedAtUtc)
    {
        if (Status == ConditionStatus.Waived)
        {
            return;
        }

        Status = ConditionStatus.Waived;
        CompletedAtUtc = completedAtUtc;
    }

    public void MarkFailed()
    {
        if (Status == ConditionStatus.Failed)
        {
            return;
        }

        Status = ConditionStatus.Failed;
        CompletedAtUtc = null;
    }

    public void MarkExpiredIfOverdue(DateOnly today)
    {
        if (Status != ConditionStatus.Pending)
        {
            return;
        }

        if (today <= DueDate)
        {
            return;
        }

        Status = ConditionStatus.Expired;
        CompletedAtUtc = null;
    }

    public void MarkCompleted(DateTime completedAtUtc)
    {
        // Backward-compatible alias for existing complete endpoint semantics.
        MarkSatisfied(completedAtUtc);
    }

    public bool IsBlocking()
    {
        return Status is ConditionStatus.Pending or ConditionStatus.Failed or ConditionStatus.Expired;
    }

    public bool IsClosed()
    {
        return Status is ConditionStatus.Satisfied or ConditionStatus.Waived;
    }

    public bool IsOpen()
    {
        return !IsClosed();
    }
}
