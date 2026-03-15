using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Conditions.CompleteCondition;

public sealed class CompleteConditionHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<CompleteConditionCommand, ConditionDto?>
{
    public async Task<ConditionDto?> Handle(CompleteConditionCommand request, CancellationToken cancellationToken)
    {
        var condition = await dbContext.GetConditionByIdAsync(request.Id, cancellationToken);

        if (condition is null)
        {
            return null;
        }

        var property = await dbContext.GetPropertyByIdAsync(condition.PropertyId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        condition.MarkCompleted(dateTimeProvider.UtcNow);
        property.RecalculateStatus();

        await dbContext.SaveChangesAsync(cancellationToken);

        return ConditionDto.FromCondition(condition);
    }
}
