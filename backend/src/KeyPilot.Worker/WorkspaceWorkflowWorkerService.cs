using KeyPilot.Infrastructure.Workflow;
using KeyPilot.Workflows;
using Microsoft.Extensions.Options;
using Temporalio.Client;
using Temporalio.Worker;

namespace KeyPilot.Worker;

public sealed class WorkspaceWorkflowWorkerService(
    IOptions<TemporalOptions> options,
    IServiceScopeFactory scopeFactory,
    ILogger<WorkspaceWorkflowWorkerService> logger) : BackgroundService
{
    private readonly TemporalOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            logger.LogInformation("Temporal worker disabled by configuration.");
            return;
        }

        var client = await TemporalClient.ConnectAsync(new TemporalClientConnectOptions(_options.HostPort)
        {
            Namespace = _options.Namespace
        });

        var activities = new WorkspaceWorkflowActivities(scopeFactory);

        using var worker = new TemporalWorker(client, new TemporalWorkerOptions(_options.TaskQueue)
            .AddWorkflow<WorkspaceWorkflow>()
            .AddActivity(activities.ProcessReminderTickAsync));

        logger.LogInformation("Temporal worker started on task queue {TaskQueue}", _options.TaskQueue);

        await worker.ExecuteAsync(stoppingToken);
    }
}
