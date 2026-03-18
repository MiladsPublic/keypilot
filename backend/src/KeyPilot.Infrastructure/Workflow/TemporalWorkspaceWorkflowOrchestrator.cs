using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Domain.Properties;
using KeyPilot.Workflows;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Temporalio.Client;

namespace KeyPilot.Infrastructure.Workflow;

public sealed class TemporalWorkspaceWorkflowOrchestrator(
    ITemporalClientProvider clientProvider,
    IOptions<TemporalOptions> options,
    ILogger<TemporalWorkspaceWorkflowOrchestrator> logger) : IWorkspaceWorkflowOrchestrator
{
    private readonly TemporalOptions _options = options.Value;

    public async Task StartAsync(WorkspaceWorkflowStartInput input, CancellationToken cancellationToken)
    {
        var client = await clientProvider.GetClientAsync(cancellationToken);

        if (client is null)
        {
            return;
        }

        var workflowId = WorkflowId(input.WorkspaceId);
        var reminders = input.PendingConditionDueDates
            .Select(date => date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(9))
            .ToArray();
        var buyingMethod = ToWorkflowBuyingMethod(input.BuyingMethod);
        DateTime? methodSpecificReminderAtUtc = input.BuyingMethod switch
        {
            BuyingMethod.Auction when input.AcceptedOfferDate.HasValue =>
                input.AcceptedOfferDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(-1).AddHours(9),
            BuyingMethod.Deadline when input.SettlementDate.HasValue =>
                input.SettlementDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(-1).AddHours(9),
            _ => null
        };

        var settlementReminderAtUtc = input.SettlementDate.HasValue
            ? input.SettlementDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddHours(9)
            : (DateTime?)null;

        if (settlementReminderAtUtc is null)
        {
            return;
        }

        try
        {
            await client.StartWorkflowAsync(
                (WorkspaceWorkflow workflow) => workflow.RunAsync(
                    new WorkspaceWorkflowInput(
                        input.WorkspaceId,
                        input.PropertyId,
                        input.OwnerUserId,
                        buyingMethod,
                        methodSpecificReminderAtUtc,
                        settlementReminderAtUtc.Value,
                        reminders)),
                new WorkflowOptions(
                    id: workflowId,
                    taskQueue: _options.TaskQueue));
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("already", StringComparison.OrdinalIgnoreCase) &&
                ex.Message.Contains("start", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogDebug("Workspace workflow already started for {WorkspaceId}", input.WorkspaceId);
                return;
            }

            throw;
        }
    }

    public async Task SignalAsync(WorkspaceWorkflowSignal signal, CancellationToken cancellationToken)
    {
        var client = await clientProvider.GetClientAsync(cancellationToken);

        if (client is null)
        {
            return;
        }

        var handle = client.GetWorkflowHandle<WorkspaceWorkflow>(WorkflowId(signal.WorkspaceId));

        await handle.SignalAsync(
            workflow => workflow.SignalWorkspaceEventAsync(
                new WorkspaceWorkflowSignalInput(
                    signal.EventType,
                    signal.OccurredAtUtc,
                    signal.TaskId,
                    signal.ConditionId)));
    }

    private static string WorkflowId(Guid workspaceId)
    {
        return $"workspace-{workspaceId}";
    }

    private static string ToWorkflowBuyingMethod(BuyingMethod buyingMethod)
    {
        return buyingMethod switch
        {
            BuyingMethod.PrivateSale => "private_sale",
            BuyingMethod.Auction => "auction",
            BuyingMethod.Negotiation => "negotiation",
            BuyingMethod.Tender => "tender",
            BuyingMethod.Deadline => "deadline",
            _ => "private_sale"
        };
    }
}
