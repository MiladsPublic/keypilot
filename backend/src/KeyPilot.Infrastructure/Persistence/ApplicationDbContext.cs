using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Domain.Properties;
using Microsoft.EntityFrameworkCore;

namespace KeyPilot.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Condition> Conditions => Set<Condition>();
    public DbSet<PropertyTask> Tasks => Set<PropertyTask>();

    public async Task AddPropertyAsync(Property property, CancellationToken cancellationToken)
    {
        await Properties.AddAsync(property, cancellationToken);
    }

    public Task<Property?> GetPropertyByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Properties
            .Include(property => property.Conditions)
            .Include(property => property.Tasks)
            .SingleOrDefaultAsync(property => property.Id == id, cancellationToken);
    }

    public Task<Condition?> GetConditionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Conditions.SingleOrDefaultAsync(condition => condition.Id == id, cancellationToken);
    }

    public Task<PropertyTask?> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Tasks.SingleOrDefaultAsync(task => task.Id == id, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
