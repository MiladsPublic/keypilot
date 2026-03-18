using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Application.Properties.Summary;
using MediatR;

namespace KeyPilot.Application.Properties.ArchiveProperty;

public sealed class ArchivePropertyHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IWorkspaceSummaryService workspaceSummaryService)
    : IRequestHandler<ArchivePropertyCommand, PropertyDto?>
{
    public async Task<PropertyDto?> Handle(ArchivePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        property.MarkArchived();
        await dbContext.SaveChangesAsync(cancellationToken);

        return workspaceSummaryService.BuildPropertyDto(property, DateOnly.FromDateTime(dateTimeProvider.UtcNow));
    }
}
