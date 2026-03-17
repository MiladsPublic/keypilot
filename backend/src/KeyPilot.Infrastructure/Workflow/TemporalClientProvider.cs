using Microsoft.Extensions.Options;
using Temporalio.Client;

namespace KeyPilot.Infrastructure.Workflow;

public sealed class TemporalClientProvider(IOptions<TemporalOptions> options) : ITemporalClientProvider
{
    private readonly TemporalOptions _options = options.Value;
    private Task<ITemporalClient?>? _clientTask;
    private readonly object _sync = new();

    public Task<ITemporalClient?> GetClientAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return Task.FromResult<ITemporalClient?>(null);
        }

        lock (_sync)
        {
            _clientTask ??= ConnectAsync(cancellationToken);
            return _clientTask;
        }
    }

    private async Task<ITemporalClient?> ConnectAsync(CancellationToken cancellationToken)
    {
        var client = await TemporalClient.ConnectAsync(new TemporalClientConnectOptions(_options.HostPort)
        {
            Namespace = _options.Namespace
        });

        cancellationToken.ThrowIfCancellationRequested();
        return client;
    }
}
