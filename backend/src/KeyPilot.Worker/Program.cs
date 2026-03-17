using KeyPilot.Application;
using KeyPilot.Infrastructure;
using KeyPilot.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddHostedService<WorkspaceWorkflowWorkerService>();

var host = builder.Build();
await host.RunAsync();
