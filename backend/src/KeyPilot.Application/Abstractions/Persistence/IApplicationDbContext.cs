using KeyPilot.Domain.Properties;
using KeyPilot.Domain.Workflows;

namespace KeyPilot.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    Task AddPropertyAsync(Property property, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Property>> ListPropertiesByOwnerAsync(string ownerUserId, CancellationToken cancellationToken);

    Task<Property?> GetPropertyByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken);

    Task<Property?> GetPropertyByWorkspaceIdAsync(Guid workspaceId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<WorkspaceReminder>> ListRemindersByPropertyAsync(Guid propertyId, CancellationToken cancellationToken);

    Task<Condition?> GetConditionByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken);

    Task<PropertyTask?> GetTaskByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken);

    void AddWorkflowEvent(WorkspaceWorkflowEvent workflowEvent);

    Task<bool> WorkflowEventExistsByDeduplicationKeyAsync(string deduplicationKey, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<WorkspaceWorkflowEvent>> ListPendingWorkflowEventsAsync(int take, CancellationToken cancellationToken);

    Task<WorkspaceWorkflowInstance?> GetWorkflowInstanceByWorkspaceIdAsync(Guid workspaceId, CancellationToken cancellationToken);

    Task AddWorkflowInstanceAsync(WorkspaceWorkflowInstance workflowInstance, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
