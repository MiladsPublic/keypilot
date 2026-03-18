using KeyPilot.Domain.Common;

namespace KeyPilot.Domain.Properties;

public sealed class Property : AuditableEntity
{
    private readonly List<Condition> _conditions = [];
    private readonly List<WorkspaceReminder> _reminders = [];
    private readonly List<PropertyTask> _tasks = [];
    private readonly List<Document> _documents = [];
    private readonly List<Contact> _contacts = [];

    private Property()
    {
    }

    private Property(
        string address,
        DateOnly acceptedOfferDate,
        DateOnly settlementDate,
        decimal? purchasePrice,
        decimal? depositAmount,
        BuyingMethod buyingMethod,
        string ownerUserId,
        Guid? workspaceId,
        DateTime createdAtUtc)
    {
        Address = address;
        AcceptedOfferDate = acceptedOfferDate;
        SettlementDate = settlementDate;
        PurchasePrice = purchasePrice;
        DepositAmount = depositAmount;
        BuyingMethod = buyingMethod;
        OwnerUserId = ownerUserId;
        WorkspaceId = workspaceId;
        Status = PropertyStatus.Conditional;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid? WorkspaceId { get; private set; }

    public string OwnerUserId { get; private set; } = string.Empty;

    public string Address { get; private set; } = string.Empty;

    public PropertyStatus Status { get; private set; } = PropertyStatus.AcceptedOffer;

    public DateOnly AcceptedOfferDate { get; private set; }

    public DateOnly SettlementDate { get; private set; }

    public decimal? PurchasePrice { get; private set; }

    public decimal? DepositAmount { get; private set; }

    public BuyingMethod BuyingMethod { get; private set; } = BuyingMethod.PrivateSale;

    public DateOnly? UnconditionalDate { get; private set; }

    public DateOnly? SettledDate { get; private set; }

    public DateOnly? CancelledDate { get; private set; }

    public IReadOnlyCollection<Condition> Conditions => _conditions;

    public IReadOnlyCollection<WorkspaceReminder> Reminders => _reminders;

    public IReadOnlyCollection<PropertyTask> Tasks => _tasks;

    public IReadOnlyCollection<Document> Documents => _documents;

    public IReadOnlyCollection<Contact> Contacts => _contacts;

    public static Property Create(
        string address,
        DateOnly acceptedOfferDate,
        DateOnly settlementDate,
        decimal? purchasePrice,
        decimal? depositAmount,
        string ownerUserId,
        Guid? workspaceId,
        DateTime createdAtUtc,
        BuyingMethod buyingMethod = BuyingMethod.PrivateSale)
    {
        return new Property(
            address,
            acceptedOfferDate,
            settlementDate,
            purchasePrice,
            depositAmount,
            buyingMethod,
            ownerUserId,
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

    public WorkspaceReminder AddReminder(
        string key,
        string title,
        DateTime scheduledForUtc,
        DateTime createdAtUtc)
    {
        var reminder = WorkspaceReminder.Create(Id, key, title, scheduledForUtc, createdAtUtc);
        _reminders.Add(reminder);
        return reminder;
    }

    public Document AddDocument(
        string storageKey,
        string fileName,
        string category,
        DateTime createdAtUtc)
    {
        var document = new Document(Id, storageKey, fileName, category, createdAtUtc);
        _documents.Add(document);
        return document;
    }

    public Contact AddContact(
        string role,
        string name,
        string? email,
        string? phone,
        DateTime createdAtUtc)
    {
        var contact = new Contact(Id, role, name, email, phone, createdAtUtc);
        _contacts.Add(contact);
        return contact;
    }

    public void MarkSettlementComplete(DateTime settledAtUtc)
    {
        SettledDate = DateOnly.FromDateTime(settledAtUtc);
        RecalculateStatus(DateOnly.FromDateTime(settledAtUtc));
    }

    public void MarkCancelled(DateTime cancelledAtUtc)
    {
        CancelledDate = DateOnly.FromDateTime(cancelledAtUtc);
        Status = PropertyStatus.Cancelled;
    }

    public void RecalculateStatus(DateOnly today)
    {
        if (CancelledDate.HasValue)
        {
            Status = PropertyStatus.Cancelled;
            return;
        }

        if (SettledDate.HasValue)
        {
            Status = PropertyStatus.Settled;
            return;
        }

        foreach (var condition in _conditions)
        {
            condition.MarkExpiredIfOverdue(today);
        }

        var hasBlockingConditions = _conditions.Any(condition => condition.IsBlocking());

        if (hasBlockingConditions)
        {
            Status = PropertyStatus.Conditional;
            return;
        }

        if (_conditions.Count == 0 || _conditions.All(condition => condition.IsClosed()))
        {
            if (!UnconditionalDate.HasValue)
            {
                UnconditionalDate = today;
            }

            Status = PropertyStatus.Unconditional;
            return;
        }

        Status = PropertyStatus.Conditional;
    }

    public void RecalculateStatus()
    {
        RecalculateStatus(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
