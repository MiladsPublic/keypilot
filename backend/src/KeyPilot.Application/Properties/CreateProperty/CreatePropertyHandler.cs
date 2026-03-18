using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Application.Properties.Lifecycle;
using KeyPilot.Application.Properties.Reminders;
using KeyPilot.Application.Properties.Summary;
using KeyPilot.Application.Properties.TaskGeneration;
using KeyPilot.Domain.Properties;
using MediatR;

namespace KeyPilot.Application.Properties.CreateProperty;

public sealed class CreatePropertyHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IWorkspaceWorkflowOutbox workflowOutbox,
    IWorkspaceLifecycleService workspaceLifecycleService,
    IWorkspaceReminderSyncService workspaceReminderSyncService,
    IWorkspaceSummaryService workspaceSummaryService,
    ITaskTemplateService taskTemplateService,
    ISettlementChecklistGenerator settlementChecklistGenerator) : IRequestHandler<CreatePropertyCommand, CreatePropertyResponse>
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

    public async Task<CreatePropertyResponse> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var createdAtUtc = dateTimeProvider.UtcNow;
        var buyingMethod = ParseBuyingMethod(request.BuyingMethod);
        var hasAcceptedOffer = request.AcceptedOfferDate.HasValue;
        var initialStatus = hasAcceptedOffer ? PropertyStatus.Conditional : PropertyStatus.Discovery;

        var property = Property.Create(
            request.Address.Trim(),
            request.AcceptedOfferDate,
            request.SettlementDate,
            request.PurchasePrice,
            request.DepositAmount,
            request.OwnerUserId,
            workspaceId: Guid.NewGuid(),
            createdAtUtc: createdAtUtc,
            buyingMethod: buyingMethod,
            methodReference: request.MethodReference?.Trim(),
            initialStatus: initialStatus);

        if (hasAcceptedOffer)
        {
            foreach (var template in taskTemplateService.GetAcceptedOfferTasks(buyingMethod))
            {
                property.AddTask(template.Title, TaskStage.Submitted, null, conditionId: null, createdAtUtc,
                    description: template.Description, importance: TaskImportance.Mandatory);
            }

            var selectedConditions = request.Conditions ?? [];

            foreach (var conditionInput in selectedConditions)
            {
                var conditionType = ParseConditionType(conditionInput.Type);
                var dueDate = conditionInput.DueDate
                    ?? request.AcceptedOfferDate!.Value.AddDays(
                        conditionInput.DaysFromAcceptedOffer
                        ?? DefaultConditionOffsets[conditionType]);

                var condition = property.AddCondition(conditionType, dueDate, createdAtUtc);

                foreach (var condTemplate in taskTemplateService.GetConditionTasks(conditionType))
                {
                    var taskDueDate = dueDate.AddDays(GetConditionTaskOffset(condTemplate.Title));
                    property.AddTask(condTemplate.Title, TaskStage.Conditional, taskDueDate, condition.Id, createdAtUtc,
                        description: condTemplate.Description, importance: TaskImportance.Mandatory);
                }
            }

            workspaceLifecycleService.ApplyDerivedState(property, DateOnly.FromDateTime(createdAtUtc));
            settlementChecklistGenerator.EnsureGenerated(property, createdAtUtc);
            await workspaceReminderSyncService.SyncAsync(property, createdAtUtc, cancellationToken);
        }
        else
        {
            foreach (var template in taskTemplateService.GetDiscoveryTasks(buyingMethod))
            {
                property.AddTask(template.Title, TaskStage.Discovery, null, conditionId: null, createdAtUtc,
                    description: template.Description, importance: TaskImportance.Recommended);
            }
        }

        await dbContext.AddPropertyAsync(property, cancellationToken);
        await workflowOutbox.EnqueueWorkspaceCreatedAsync(property, createdAtUtc, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return workspaceSummaryService.BuildCreateResponse(property, DateOnly.FromDateTime(createdAtUtc));
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

    private static BuyingMethod ParseBuyingMethod(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "private_sale" => BuyingMethod.PrivateSale,
            "auction" => BuyingMethod.Auction,
            "negotiation" => BuyingMethod.Negotiation,
            "tender" => BuyingMethod.Tender,
            "deadline" => BuyingMethod.Deadline,
            _ => throw new ArgumentOutOfRangeException(nameof(value), "Unsupported buying method.")
        };
    }
}
