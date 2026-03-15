using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    Task AddPropertyAsync(Property property, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Property>> ListPropertiesByOwnerAsync(string ownerUserId, CancellationToken cancellationToken);

    Task<Property?> GetPropertyByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken);

    Task<Condition?> GetConditionByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken);

    Task<PropertyTask?> GetTaskByIdAsync(Guid id, string ownerUserId, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
