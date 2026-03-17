using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.Lifecycle;
using KeyPilot.Application.Properties.Summary;
using MediatR;

namespace KeyPilot.Application.Properties.GetPropertyById;

public sealed class GetPropertyByIdHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IWorkspaceLifecycleService workspaceLifecycleService,
    IWorkspaceSummaryService workspaceSummaryService)
    : IRequestHandler<GetPropertyByIdQuery, PropertyDto?>
{
    public async Task<PropertyDto?> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow);

        if (property is not null)
        {
            workspaceLifecycleService.ApplyDerivedState(property, today);
        }

        return property is null ? null : workspaceSummaryService.BuildPropertyDto(property, today);
    }
}
