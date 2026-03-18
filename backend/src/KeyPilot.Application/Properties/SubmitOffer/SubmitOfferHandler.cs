using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Application.Properties.Lifecycle;
using KeyPilot.Application.Properties.Reminders;
using KeyPilot.Application.Properties.Summary;
using KeyPilot.Application.Properties.TaskGeneration;
using KeyPilot.Domain.Properties;
using MediatR;

namespace KeyPilot.Application.Properties.SubmitOffer;

public sealed class SubmitOfferHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IWorkspaceWorkflowOutbox workflowOutbox,
    IWorkspaceLifecycleService workspaceLifecycleService,
    IWorkspaceReminderSyncService workspaceReminderSyncService,
    IWorkspaceSummaryService workspaceSummaryService,
    ITaskTemplateService taskTemplateService,
    ISettlementChecklistGenerator settlementChecklistGenerator)
    : IRequestHandler<SubmitOfferCommand, PropertyDto?>
{
    private static readonly IReadOnlyDictionary<ConditionType, int> DefaultConditionOffsets =
        new Dictionary<ConditionType, int>
        {
            [ConditionType.Finance] = 5,
            [ConditionType.BuildingReport] = 5,
            [ConditionType.Lim] = 10,
            [ConditionType.Insurance] = 10,
            [ConditionType.SolicitorApproval] = 5
        };

    public async Task<PropertyDto?> Handle(SubmitOfferCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        if (property.AcceptedOfferDate.HasValue)
        {
            return null;
        }

        var now = dateTimeProvider.UtcNow;
        var today = DateOnly.FromDateTime(now);

        property.Update(
            address: null,
            acceptedOfferDate: request.AcceptedOfferDate,
            settlementDate: request.SettlementDate,
            purchasePrice: null,
            depositAmount: null,
            methodReference: null);

        foreach (var template in taskTemplateService.GetAcceptedOfferTasks(property.BuyingMethod))
        {
            property.AddTask(template.Title, TaskStage.Submitted, null, conditionId: null, now,
                description: template.Description, importance: TaskImportance.Mandatory);
        }

        var selectedConditions = request.Conditions ?? [];

        foreach (var conditionInput in selectedConditions)
        {
            var conditionType = ParseConditionType(conditionInput.Type);
            var dueDate = conditionInput.DueDate
                ?? request.AcceptedOfferDate.AddDays(
                    conditionInput.DaysFromAcceptedOffer
                    ?? DefaultConditionOffsets[conditionType]);

            var condition = property.AddCondition(conditionType, dueDate, now);

            foreach (var condTemplate in taskTemplateService.GetConditionTasks(conditionType))
            {
                var taskDueDate = dueDate.AddDays(GetConditionTaskOffset(condTemplate.Title));
                property.AddTask(condTemplate.Title, TaskStage.Conditional, taskDueDate, condition.Id, now,
                    description: condTemplate.Description, importance: TaskImportance.Mandatory);
            }
        }

        workspaceLifecycleService.ApplyDerivedState(property, today);
        settlementChecklistGenerator.EnsureGenerated(property, now);
        await workspaceReminderSyncService.SyncAsync(property, now, cancellationToken);

        if (property.WorkspaceId.HasValue)
        {
            await workflowOutbox.EnqueueWorkspaceSignalAsync(
                property.WorkspaceId.Value,
                eventType: "offer_submitted",
                occurredAtUtc: now,
                cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return workspaceSummaryService.BuildPropertyDto(property, today);
    }

    private static ConditionType ParseConditionType(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "finance" => ConditionType.Finance,
            "building_report" => ConditionType.BuildingReport,
            "lim" => ConditionType.Lim,
            "insurance" => ConditionType.Insurance,
            "solicitor_approval" => ConditionType.SolicitorApproval,
            _ => throw new ArgumentOutOfRangeException(nameof(value), "Unsupported condition type.")
        };
    }

    private static int GetConditionTaskOffset(string title)
    {
        return title.Trim().ToLowerInvariant() switch
        {
            "submit finance documents" => -2,
            "confirm lender approval" => -1,
            "book building inspection" => -3,
            "review building report" => -1,
            "obtain lim report" => -3,
            "review lim findings" => -1,
            _ => 0
        };
    }
}
