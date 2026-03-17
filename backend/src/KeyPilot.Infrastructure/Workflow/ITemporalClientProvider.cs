using Temporalio.Client;

namespace KeyPilot.Infrastructure.Workflow;

public interface ITemporalClientProvider
{
    Task<ITemporalClient?> GetClientAsync(CancellationToken cancellationToken);
}
