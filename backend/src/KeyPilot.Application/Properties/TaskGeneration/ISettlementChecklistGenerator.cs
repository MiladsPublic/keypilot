using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.TaskGeneration;

public interface ISettlementChecklistGenerator
{
    void EnsureGenerated(Property property, DateTime createdAtUtc);
}
