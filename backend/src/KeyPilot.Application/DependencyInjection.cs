using FluentValidation;
using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Application.Properties.Lifecycle;
using KeyPilot.Application.Properties.Reminders;
using KeyPilot.Application.Properties.Summary;
using KeyPilot.Application.Properties.TaskGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace KeyPilot.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<ITaskTemplateService, TaskTemplateService>();
        services.AddScoped<ISettlementChecklistGenerator, SettlementChecklistGenerator>();
        services.AddScoped<IWorkspaceLifecycleService, WorkspaceLifecycleService>();
        services.AddScoped<IWorkspaceSummaryService, WorkspaceSummaryService>();
        services.AddScoped<IWorkspaceReminderSyncService, WorkspaceReminderSyncService>();
        services.AddScoped<IWorkspaceWorkflowOrchestrator, NoopWorkspaceWorkflowOrchestrator>();

        return services;
    }
}
