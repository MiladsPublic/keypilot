using KeyPilot.Domain.Common;

namespace KeyPilot.Domain.Properties;

public sealed class Property : AuditableEntity
{
    private readonly List<Condition> _conditions = [];
    private readonly List<PropertyTask> _tasks = [];

    private Property()
    {
    }

    private Property(
        string address,
        DateOnly acceptedOfferDate,
        DateOnly settlementDate,
        decimal? purchasePrice,
        decimal? depositAmount,
        Guid? workspaceId,
        DateTime createdAtUtc)
    {
        Address = address;
        AcceptedOfferDate = acceptedOfferDate;
        SettlementDate = settlementDate;
        PurchasePrice = purchasePrice;
        DepositAmount = depositAmount;
        WorkspaceId = workspaceId;
        Status = PropertyStatus.AcceptedOffer;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid? WorkspaceId { get; private set; }

    public string Address { get; private set; } = string.Empty;

    public PropertyStatus Status { get; private set; } = PropertyStatus.AcceptedOffer;

    public DateOnly AcceptedOfferDate { get; private set; }

    public DateOnly SettlementDate { get; private set; }

    public decimal? PurchasePrice { get; private set; }

    public decimal? DepositAmount { get; private set; }

    public IReadOnlyCollection<Condition> Conditions => _conditions;

    public IReadOnlyCollection<PropertyTask> Tasks => _tasks;

    public static Property Create(
        string address,
        DateOnly acceptedOfferDate,
        DateOnly settlementDate,
        decimal? purchasePrice,
        decimal? depositAmount,
        Guid? workspaceId,
        DateTime createdAtUtc)
    {
        return new Property(
            address,
            acceptedOfferDate,
            settlementDate,
            purchasePrice,
            depositAmount,
            workspaceId,
            createdAtUtc);
    }

    public Condition AddCondition(ConditionType type, DateOnly dueDate, DateTime createdAtUtc)
    {
        var condition = new Condition(Id, type, dueDate, createdAtUtc);
        _conditions.Add(condition);
        return condition;
    }

    public PropertyTask AddTask(
        string title,
        TaskStage stage,
        DateOnly? dueDate,
        Guid? conditionId,
        DateTime createdAtUtc)
    {
        var task = new PropertyTask(Id, conditionId, title, stage, dueDate, createdAtUtc);
        _tasks.Add(task);
        return task;
    }

    public void MarkSettlementComplete()
    {
        Status = PropertyStatus.Settled;
    }

    public void RecalculateStatus()
    {
        if (Status == PropertyStatus.Settled)
        {
            return;
        }

        var hasPendingConditions = _conditions.Any(condition => condition.Status == ConditionStatus.Pending);

        if (hasPendingConditions)
        {
            Status = PropertyStatus.Conditional;
            return;
        }

        var hasPreSettlementTasks = _tasks.Any(task =>
            task.Stage == TaskStage.PreSettlement && task.Status == TaskStatus.Pending);

        Status = hasPreSettlementTasks
            ? PropertyStatus.PreSettlement
            : PropertyStatus.Unconditional;
    }
}
