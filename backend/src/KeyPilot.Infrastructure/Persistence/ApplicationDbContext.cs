using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Domain.Properties;
using Microsoft.EntityFrameworkCore;

namespace KeyPilot.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Property> Properties => Set<Property>();

    public async Task AddPropertyAsync(Property property, CancellationToken cancellationToken)
    {
        await Properties.AddAsync(property, cancellationToken);
    }

    public Task<Property?> GetPropertyByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Properties.SingleOrDefaultAsync(property => property.Id == id, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
