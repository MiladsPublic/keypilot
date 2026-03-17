using KeyPilot.Application.Properties.CreateProperty;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Summary;

public interface IWorkspaceSummaryService
{
    PropertyDto BuildPropertyDto(Property workspace, DateOnly today);

    CreatePropertyResponse BuildCreateResponse(Property workspace, DateOnly today);
}