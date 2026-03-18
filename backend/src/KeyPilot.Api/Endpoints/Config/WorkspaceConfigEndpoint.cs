namespace KeyPilot.Api.Endpoints.Config;

public static class WorkspaceConfigEndpoint
{
    public sealed record ConditionDefault(string Type, string Label, int DaysFromAcceptedOffer);

    public sealed record BuyingMethodOption(string Value, string Label);

    public sealed record MethodProfile(
        string Description,
        IReadOnlyCollection<string> Stages,
        IReadOnlyDictionary<string, string> StageLabels,
        int TypicalSettlementDays);

    public sealed record WorkspaceConfigResponse(
        IReadOnlyCollection<ConditionDefault> ConditionDefaults,
        IReadOnlyCollection<BuyingMethodOption> BuyingMethods,
        IReadOnlyDictionary<string, IReadOnlyCollection<ConditionDefault>> ConditionsByMethod,
        IReadOnlyDictionary<string, MethodProfile> MethodProfiles);

    private static readonly IReadOnlyCollection<ConditionDefault> AllConditions =
    [
        new("finance", "Finance", 5),
        new("building_report", "Building report", 5),
        new("lim", "LIM", 10),
        new("insurance", "Insurance", 10),
        new("solicitor_approval", "Solicitor approval", 5)
    ];

    private static readonly IReadOnlyCollection<ConditionDefault> NoConditions = [];

    private static readonly IReadOnlyDictionary<string, IReadOnlyCollection<ConditionDefault>> ConditionsByMethod =
        new Dictionary<string, IReadOnlyCollection<ConditionDefault>>
        {
            ["private_sale"] = AllConditions,
            ["auction"] = NoConditions,
            ["negotiation"] = AllConditions,
            ["tender"] =
            [
                new("solicitor_approval", "Solicitor approval", 5)
            ],
            ["deadline"] = AllConditions
        };

    private static readonly string[] FullPipeline =
        ["accepted_offer", "conditional", "unconditional", "pre_settlement", "settlement"];

    private static readonly string[] AuctionPipeline =
        ["accepted_offer", "unconditional", "pre_settlement", "settlement"];

    private static readonly IReadOnlyDictionary<string, string> NoLabels =
        new Dictionary<string, string>();

    private static readonly IReadOnlyDictionary<string, MethodProfile> MethodProfiles =
        new Dictionary<string, MethodProfile>
        {
            ["private_sale"] = new(
                Description: "A standard conditional sale with negotiated terms. Conditions apply and settlement typically follows after they're satisfied.",
                Stages: FullPipeline,
                StageLabels: NoLabels,
                TypicalSettlementDays: 30),
            ["auction"] = new(
                Description: "Unconditional purchase at auction. No conditions apply \u2014 the sale is binding from the fall of the hammer.",
                Stages: AuctionPipeline,
                StageLabels: new Dictionary<string, string> { ["accepted_offer"] = "Auction Won" },
                TypicalSettlementDays: 20),
            ["negotiation"] = new(
                Description: "Price and terms are negotiated directly with the vendor. Standard conditions can apply.",
                Stages: FullPipeline,
                StageLabels: NoLabels,
                TypicalSettlementDays: 30),
            ["tender"] = new(
                Description: "A sealed-bid process with a fixed closing date. Usually limited conditions \u2014 solicitor approval is common.",
                Stages: FullPipeline,
                StageLabels: new Dictionary<string, string> { ["accepted_offer"] = "Tender Accepted" },
                TypicalSettlementDays: 25),
            ["deadline"] = new(
                Description: "Offers submitted by a set deadline. The vendor reviews all offers and selects one. Standard conditions can apply.",
                Stages: FullPipeline,
                StageLabels: new Dictionary<string, string> { ["accepted_offer"] = "Deadline Accepted" },
                TypicalSettlementDays: 30),
        };

    private static readonly WorkspaceConfigResponse Config = new(
        ConditionDefaults: AllConditions,
        BuyingMethods:
        [
            new("private_sale", "Private Sale"),
            new("auction", "Auction"),
            new("negotiation", "Negotiation"),
            new("tender", "Tender"),
            new("deadline", "Deadline Sale")
        ],
        ConditionsByMethod: ConditionsByMethod,
        MethodProfiles: MethodProfiles);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/config/workspace", () => TypedResults.Ok(Config))
            .WithName("GetWorkspaceConfig")
            .WithTags("Config")
            .WithSummary("Returns condition defaults and buying method options for workspace creation.")
            .Produces<WorkspaceConfigResponse>();
    }
}
