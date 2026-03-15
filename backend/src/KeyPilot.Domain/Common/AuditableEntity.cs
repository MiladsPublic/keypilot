namespace KeyPilot.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAtUtc { get; protected set; }
}
