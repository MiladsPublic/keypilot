using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Infrastructure.Persistence;
using KeyPilot.Infrastructure.Time;
using KeyPilot.Infrastructure.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KeyPilot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5433;Database=keypilot;Username=keypilot;Password=keypilot";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        services.AddScoped<IApplicationDbContext>(serviceProvider =>
            serviceProvider.GetRequiredService<ApplicationDbContext>());
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.Configure<TemporalOptions>(options =>
            configuration.GetSection(TemporalOptions.SectionName).Bind(options));
        services.AddSingleton<ITemporalClientProvider, TemporalClientProvider>();
        services.AddScoped<IWorkspaceWorkflowOrchestrator, TemporalWorkspaceWorkflowOrchestrator>();

        return services;
    }
}
