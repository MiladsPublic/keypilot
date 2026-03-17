using KeyPilot.Application.Properties.CreateProperty;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Summary;

internal sealed class WorkspaceSummaryService : IWorkspaceSummaryService
{
    public PropertyDto BuildPropertyDto(Property workspace, DateOnly today)
    {
        return PropertyDto.FromProperty(workspace, today);
    }

    public CreatePropertyResponse BuildCreateResponse(Property workspace, DateOnly today)
    {
        return CreatePropertyResponse.FromProperty(workspace, today);
    }
}