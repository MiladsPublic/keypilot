using FluentAssertions;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.Lifecycle;
using KeyPilot.Application.Properties.Reminders;
using KeyPilot.Application.Properties.Reminders.ProcessWorkspaceReminderTick;
using KeyPilot.Domain.Properties;
using KeyPilot.Domain.Workflows;

namespace KeyPilot.Api.Tests;

public sealed class ProcessWorkspaceReminderTickHandlerTests
{
    [Fact]
    public async Task Handle_MarksDueReminderSent_AndExpiresOverdueCondition()
    {
        var now = new DateTime(2026, 3, 17, 9, 0, 0, DateTimeKind.Utc);
        var accepted = DateOnly.FromDateTime(now.AddDays(-3));
        var workspaceId = Guid.NewGuid();

        var workspace = Property.Create(
            "7 Test Street",
            accepted,
            accepted.AddDays(14),
            null,
            null,
            "user-1",
            workspaceId,
            createdAtUtc: now.AddDays(-3));

        var overdueCondition = workspace.AddCondition(ConditionType.Insurance, accepted.AddDays(1), now.AddDays(-3));
        var reminder = workspace.AddReminder("settlement:due", "Settlement date reminder", now.AddMinutes(-5), now.AddDays(-1));

        var dbContext = new FakeApplicationDbContext(workspace);
        var handler = new ProcessWorkspaceReminderTickHandler(
            dbContext,
            new WorkspaceLifecycleServiceFake(),
            new WorkspaceReminderSyncNoopService());

        var result = await handler.Handle(new ProcessWorkspaceReminderTickCommand(workspaceId, now), CancellationToken.None);

        result.Should().BeTrue();
        reminder.Status.Should().Be(WorkspaceReminderStatus.Sent);
        overdueCondition.Status.Should().Be(ConditionStatus.Expired);
        dbContext.SaveChangesCalls.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenWorkspaceNotFound()
    {
        var dbContext = new FakeApplicationDbContext(workspace: null);
        var handler = new ProcessWorkspaceReminderTickHandler(
            dbContext,
            new WorkspaceLifecycleServiceFake(),
            new WorkspaceReminderSyncNoopService());

        var result = await handler.Handle(
            new ProcessWorkspaceReminderTickCommand(Guid.NewGuid(), DateTime.UtcNow),
            CancellationToken.None);

        result.Should().BeFalse();
        dbContext.SaveChangesCalls.Should().Be(0);
    }

    private sealed class FakeApplicationDbContext(Property? workspace) : IApplicationDbContext
    {
        private readonly Property? _workspace = workspace;

        public int SaveChangesCalls { get; private set; }

        public Task AddPropertyAsync(Property property, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyCollection<Property>> ListPropertiesByOwnerAsync(string ownerUserId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<Property?> GetPropertyByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<Property?> GetPropertyByWorkspaceIdAsync(Guid workspaceId, CancellationToken cancellationToken)
        {
            if (_workspace?.WorkspaceId == workspaceId)
            {
                return Task.FromResult<Property?>(_workspace);
            }

            return Task.FromResult<Property?>(null);
        }

        public Task<IReadOnlyCollection<WorkspaceReminder>> ListRemindersByPropertyAsync(Guid propertyId, CancellationToken cancellationToken)
        {
            if (_workspace is null)
            {
                return Task.FromResult<IReadOnlyCollection<WorkspaceReminder>>(Array.Empty<WorkspaceReminder>());
            }

            return Task.FromResult<IReadOnlyCollection<WorkspaceReminder>>(_workspace.Reminders.ToArray());
        }

        public Task<Condition?> GetConditionByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<PropertyTask?> GetTaskByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public void AddWorkflowEvent(WorkspaceWorkflowEvent workflowEvent)
        {
            throw new NotSupportedException();
        }

        public Task<bool> WorkflowEventExistsByDeduplicationKeyAsync(string deduplicationKey, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyCollection<WorkspaceWorkflowEvent>> ListPendingWorkflowEventsAsync(int take, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<WorkspaceWorkflowInstance?> GetWorkflowInstanceByWorkspaceIdAsync(Guid workspaceId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task AddWorkflowInstanceAsync(WorkspaceWorkflowInstance workflowInstance, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCalls += 1;
            return Task.FromResult(1);
        }

        public Task<Document?> GetDocumentByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<Contact?> GetContactByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public void RemoveDocument(Document document)
        {
            throw new NotSupportedException();
        }

        public void RemoveContact(Contact contact)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class WorkspaceLifecycleServiceFake : IWorkspaceLifecycleService
    {
        public void ApplyDerivedState(Property workspace, DateOnly today)
        {
            workspace.RecalculateStatus(today);
        }
    }

    private sealed class WorkspaceReminderSyncNoopService : IWorkspaceReminderSyncService
    {
        public Task SyncAsync(Property workspace, DateTime nowUtc, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}