using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.Common;
using KeyPilot.Application.Properties.TaskGeneration;
using MediatR;

namespace KeyPilot.Application.Conditions.WaiveCondition;

public sealed class WaiveConditionHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ISettlementChecklistGenerator settlementChecklistGenerator) : IRequestHandler<WaiveConditionCommand, ConditionDto?>
{
    public async Task<ConditionDto?> Handle(WaiveConditionCommand request, CancellationToken cancellationToken)
    {
        var condition = await dbContext.GetConditionByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (condition is null)
        {
            return null;
        }

        var property = await dbContext.GetPropertyByIdAsync(condition.PropertyId, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        var now = dateTimeProvider.UtcNow;
        condition.MarkWaived(now);
        property.RecalculateStatus(DateOnly.FromDateTime(now));
        settlementChecklistGenerator.EnsureGenerated(property, now);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ConditionDto.FromCondition(condition);
    }
}
