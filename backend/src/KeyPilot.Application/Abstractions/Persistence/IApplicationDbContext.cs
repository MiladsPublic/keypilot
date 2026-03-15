using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    Task AddPropertyAsync(Property property, CancellationToken cancellationToken);

    Task<Property?> GetPropertyByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
