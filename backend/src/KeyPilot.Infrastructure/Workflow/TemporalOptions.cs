namespace KeyPilot.Infrastructure.Workflow;

public sealed class TemporalOptions
{
    public const string SectionName = "Temporal";

    public string HostPort { get; init; } = "localhost:7233";

    public string Namespace { get; init; } = "default";

    public string TaskQueue { get; init; } = "keypilot-workspace";

    public bool Enabled { get; init; }
}
