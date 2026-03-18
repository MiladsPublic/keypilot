using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Domain.Properties;
using KeyPilot.Domain.Workflows;
using Microsoft.EntityFrameworkCore;

namespace KeyPilot.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Condition> Conditions => Set<Condition>();
    public DbSet<WorkspaceReminder> Reminders => Set<WorkspaceReminder>();
    public DbSet<WorkspaceWorkflowEvent> WorkflowEvents => Set<WorkspaceWorkflowEvent>();
    public DbSet<WorkspaceWorkflowInstance> WorkflowInstances => Set<WorkspaceWorkflowInstance>();
    public DbSet<PropertyTask> Tasks => Set<PropertyTask>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Contact> Contacts => Set<Contact>();

    public async Task AddPropertyAsync(Property property, CancellationToken cancellationToken)
    {
        await Properties.AddAsync(property, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Property>> ListPropertiesByOwnerAsync(string ownerUserId, CancellationToken cancellationToken)
    {
        return await Properties
            .Where(property => property.OwnerUserId == ownerUserId)
            .Include(property => property.Conditions)
            .Include(property => property.Reminders)
            .Include(property => property.Tasks)
            .Include(property => property.Documents)
            .Include(property => property.Contacts)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Property?> GetPropertyByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken)
    {
        return Properties
            .Include(property => property.Conditions)
            .Include(property => property.Reminders)
            .Include(property => property.Tasks)
            .Include(property => property.Documents)
            .Include(property => property.Contacts)
            .SingleOrDefaultAsync(property => property.Id == id && property.OwnerUserId == ownerUserId, cancellationToken);
    }

    public Task<Property?> GetPropertyByWorkspaceIdAsync(Guid workspaceId, CancellationToken cancellationToken)
    {
        return Properties
            .Include(property => property.Conditions)
            .Include(property => property.Reminders)
            .Include(property => property.Tasks)
            .Include(property => property.Documents)
            .Include(property => property.Contacts)
            .SingleOrDefaultAsync(property => property.WorkspaceId == workspaceId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<WorkspaceReminder>> ListRemindersByPropertyAsync(Guid propertyId, CancellationToken cancellationToken)
    {
        return await Reminders
            .Where(reminder => reminder.PropertyId == propertyId)
            .OrderBy(reminder => reminder.ScheduledForUtc)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Condition?> GetConditionByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken)
    {
        return Conditions
            .Join(
                Properties,
                condition => condition.PropertyId,
                property => property.Id,
                (condition, property) => new { condition, property })
            .Where(result => result.condition.Id == id && result.property.OwnerUserId == ownerUserId)
            .Select(result => result.condition)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<PropertyTask?> GetTaskByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken)
    {
        return Tasks
            .Join(
                Properties,
                task => task.PropertyId,
                property => property.Id,
                (task, property) => new { task, property })
            .Where(result => result.task.Id == id && result.property.OwnerUserId == ownerUserId)
            .Select(result => result.task)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<Document?> GetDocumentByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken)
    {
        return Documents
            .Join(
                Properties,
                document => document.PropertyId,
                property => property.Id,
                (document, property) => new { document, property })
            .Where(result => result.document.Id == id && result.property.OwnerUserId == ownerUserId)
            .Select(result => result.document)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<Contact?> GetContactByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken)
    {
        return Contacts
            .Join(
                Properties,
                contact => contact.PropertyId,
                property => property.Id,
                (contact, property) => new { contact, property })
            .Where(result => result.contact.Id == id && result.property.OwnerUserId == ownerUserId)
            .Select(result => result.contact)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void RemoveDocument(Document document)
    {
        Documents.Remove(document);
    }

    public void RemoveContact(Contact contact)
    {
        Contacts.Remove(contact);
    }

    public void AddWorkflowEvent(WorkspaceWorkflowEvent workflowEvent)
    {
        WorkflowEvents.Add(workflowEvent);
    }

    public Task<bool> WorkflowEventExistsByDeduplicationKeyAsync(string deduplicationKey, CancellationToken cancellationToken)
    {
        return WorkflowEvents.AnyAsync(entity => entity.DeduplicationKey == deduplicationKey, cancellationToken);
    }

    public async Task<IReadOnlyCollection<WorkspaceWorkflowEvent>> ListPendingWorkflowEventsAsync(int take, CancellationToken cancellationToken)
    {
        return await WorkflowEvents
            .Where(entity => entity.Status == WorkspaceWorkflowEventStatus.Pending)
            .OrderBy(entity => entity.CreatedAtUtc)
            .Take(take)
            .ToArrayAsync(cancellationToken);
    }

    public Task<WorkspaceWorkflowInstance?> GetWorkflowInstanceByWorkspaceIdAsync(Guid workspaceId, CancellationToken cancellationToken)
    {
        return WorkflowInstances.SingleOrDefaultAsync(entity => entity.WorkspaceId == workspaceId, cancellationToken);
    }

    public async Task AddWorkflowInstanceAsync(WorkspaceWorkflowInstance workflowInstance, CancellationToken cancellationToken)
    {
        await WorkflowInstances.AddAsync(workflowInstance, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
