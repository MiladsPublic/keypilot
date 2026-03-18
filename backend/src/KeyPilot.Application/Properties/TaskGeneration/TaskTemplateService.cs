using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.TaskGeneration;

internal sealed class TaskTemplateService : ITaskTemplateService
{
    private static readonly IReadOnlyCollection<string> DiscoveryTasks =
    [
        "Research the neighbourhood",
        "Get mortgage pre-approval",
        "Shortlist potential properties",
        "Choose a lawyer or conveyancer"
    ];

    private static readonly IReadOnlyCollection<string> AuctionDiscoveryTasks =
    [
        "Research the neighbourhood",
        "Get mortgage pre-approval",
        "Arrange building report before auction",
        "Confirm finance is unconditional",
        "Choose a lawyer or conveyancer"
    ];

    private static readonly IReadOnlyCollection<string> AcceptedOfferTasks =
    [
        "Confirm lawyer details",
        "Review active conditions",
        "Confirm settlement date"
    ];

    private static readonly IReadOnlyCollection<string> AuctionAcceptedOfferTasks =
    [
        "Confirm lawyer details",
        "Confirm auction registration",
        "Review auction terms",
        "Confirm settlement date"
    ];

    private static readonly IReadOnlyCollection<string> TenderAcceptedOfferTasks =
    [
        "Confirm lawyer details",
        "Prepare tender submission",
        "Confirm tender deadline",
        "Confirm settlement date"
    ];

    private static readonly IReadOnlyCollection<string> DeadlineAcceptedOfferTasks =
    [
        "Confirm lawyer details",
        "Review deadline sale terms",
        "Confirm settlement date"
    ];

    private static readonly IReadOnlyCollection<string> PreSettlementTasks =
    [
        "Confirm final loan approval",
        "Confirm insurance active from settlement date",
        "Complete final inspection",
        "Confirm settlement funds ready",
        "Confirm key collection details"
    ];

    private static readonly IReadOnlyCollection<string> SettlementDayTasks =
    [
        "Lawyer confirms settlement",
        "Funds transferred",
        "Keys collected"
    ];

    public IReadOnlyCollection<string> GetDiscoveryTasks(BuyingMethod buyingMethod)
    {
        return buyingMethod switch
        {
            BuyingMethod.Auction => AuctionDiscoveryTasks,
            _ => DiscoveryTasks
        };
    }

    public IReadOnlyCollection<string> GetAcceptedOfferTasks(BuyingMethod buyingMethod)
    {
        return buyingMethod switch
        {
            BuyingMethod.Auction => AuctionAcceptedOfferTasks,
            BuyingMethod.Tender => TenderAcceptedOfferTasks,
            BuyingMethod.Deadline => DeadlineAcceptedOfferTasks,
            _ => AcceptedOfferTasks
        };
    }

    public IReadOnlyCollection<string> GetConditionTasks(ConditionType conditionType)
    {
        return conditionType switch
        {
            ConditionType.Finance =>
            [
                "Submit finance documents",
                "Confirm lender approval"
            ],
            ConditionType.BuildingReport =>
            [
                "Book building inspection",
                "Review building report"
            ],
            ConditionType.Lim =>
            [
                "Obtain LIM report",
                "Review LIM findings"
            ],
            ConditionType.Insurance =>
            [
                "Obtain insurance quote",
                "Confirm insurance cover"
            ],
            ConditionType.SolicitorApproval =>
            [
                "Send agreement to lawyer",
                "Confirm solicitor approval"
            ],
            _ => []
        };
    }

    public IReadOnlyCollection<string> GetPreSettlementTasks() => PreSettlementTasks;

    public IReadOnlyCollection<string> GetSettlementTasks() => SettlementDayTasks;
}
