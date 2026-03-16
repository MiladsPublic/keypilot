using FluentValidation;
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

        return services;
    }
}
