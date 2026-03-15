using KeyPilot.Application.Abstractions.Clock;

namespace KeyPilot.Infrastructure.Time;

internal sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
