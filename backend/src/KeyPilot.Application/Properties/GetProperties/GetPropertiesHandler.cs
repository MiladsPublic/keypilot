using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Application.Properties.Lifecycle;
using KeyPilot.Application.Properties.Summary;
using MediatR;

namespace KeyPilot.Application.Properties.GetProperties;

public sealed class GetPropertiesHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IWorkspaceLifecycleService workspaceLifecycleService,
    IWorkspaceSummaryService workspaceSummaryService)
    : IRequestHandler<GetPropertiesQuery, IReadOnlyCollection<PropertyDto>>
{
    public async Task<IReadOnlyCollection<PropertyDto>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        var properties = await dbContext.ListPropertiesByOwnerAsync(request.OwnerUserId, cancellationToken);
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow);

        foreach (var property in properties)
        {
            workspaceLifecycleService.ApplyDerivedState(property, today);
        }

        return properties
            .OrderByDescending(property => property.CreatedAtUtc)
            .Select(property => workspaceSummaryService.BuildPropertyDto(property, today))
            .ToArray();
    }
}
