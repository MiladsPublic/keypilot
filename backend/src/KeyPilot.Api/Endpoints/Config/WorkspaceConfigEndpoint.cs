namespace KeyPilot.Api.Endpoints.Config;

public static class WorkspaceConfigEndpoint
{
    public sealed record ConditionDefault(string Type, string Label, int DaysFromAcceptedOffer);

    public sealed record BuyingMethodOption(string Value, string Label);

    public sealed record WorkspaceConfigResponse(
        IReadOnlyCollection<ConditionDefault> ConditionDefaults,
        IReadOnlyCollection<BuyingMethodOption> BuyingMethods);

    private static readonly WorkspaceConfigResponse Config = new(
        ConditionDefaults:
        [
            new("finance", "Finance", 5),
            new("building_report", "Building report", 5),
            new("lim", "LIM", 10),
            new("insurance", "Insurance", 10),
            new("solicitor_approval", "Solicitor approval", 5)
        ],
        BuyingMethods:
        [
            new("private_sale", "Private Sale"),
            new("auction", "Auction"),
            new("negotiation", "Negotiation"),
            new("tender", "Tender"),
            new("deadline", "Deadline Sale")
        ]);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/config/workspace", () => TypedResults.Ok(Config))
            .WithName("GetWorkspaceConfig")
            .WithTags("Config")
            .WithSummary("Returns condition defaults and buying method options for workspace creation.")
            .Produces<WorkspaceConfigResponse>();
    }
}
