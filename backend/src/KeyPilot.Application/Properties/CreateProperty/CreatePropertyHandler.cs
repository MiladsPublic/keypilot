using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.TaskGeneration;
using KeyPilot.Domain.Properties;
using MediatR;

namespace KeyPilot.Application.Properties.CreateProperty;

public sealed class CreatePropertyHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ITaskTemplateService taskTemplateService) : IRequestHandler<CreatePropertyCommand, CreatePropertyResponse>
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
        var property = Property.Create(
            request.Address.Trim(),
            request.AcceptedOfferDate,
            request.SettlementDate,
            request.PurchasePrice,
            request.DepositAmount,
            request.OwnerUserId,
            workspaceId: null,
            createdAtUtc: createdAtUtc);

        foreach (var title in taskTemplateService.GetAcceptedOfferTasks())
        {
            property.AddTask(title, TaskStage.AcceptedOffer, null, conditionId: null, createdAtUtc);
        }

        var selectedConditions = request.Conditions ?? [];

        foreach (var conditionInput in selectedConditions)
        {
            var conditionType = ParseConditionType(conditionInput.Type);
            var dueDate = conditionInput.DueDate
                ?? request.AcceptedOfferDate.AddDays(
                    conditionInput.DaysFromAcceptedOffer
                    ?? DefaultConditionOffsets[conditionType]);

            var condition = property.AddCondition(conditionType, dueDate, createdAtUtc);

            foreach (var title in taskTemplateService.GetConditionTasks(conditionType))
            {
                var taskDueDate = dueDate.AddDays(GetConditionTaskOffset(title));
                property.AddTask(title, TaskStage.Conditional, taskDueDate, condition.Id, createdAtUtc);
            }
        }

        foreach (var title in taskTemplateService.GetPreSettlementTasks())
        {
            property.AddTask(title, TaskStage.PreSettlement, request.SettlementDate, conditionId: null, createdAtUtc);
        }

        foreach (var title in taskTemplateService.GetSettlementTasks())
        {
            property.AddTask(title, TaskStage.Settlement, request.SettlementDate, conditionId: null, createdAtUtc);
        }

        property.RecalculateStatus(DateOnly.FromDateTime(createdAtUtc));

        await dbContext.AddPropertyAsync(property, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatePropertyResponse.FromProperty(property);
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
