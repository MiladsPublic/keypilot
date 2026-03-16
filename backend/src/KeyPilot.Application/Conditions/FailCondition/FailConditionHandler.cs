using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Conditions.FailCondition;

public sealed class FailConditionHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<FailConditionCommand, ConditionDto?>
{
    public async Task<ConditionDto?> Handle(FailConditionCommand request, CancellationToken cancellationToken)
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
        condition.MarkFailed();
        property.RecalculateStatus(DateOnly.FromDateTime(now));

        await dbContext.SaveChangesAsync(cancellationToken);

        return ConditionDto.FromCondition(condition);
    }
}
