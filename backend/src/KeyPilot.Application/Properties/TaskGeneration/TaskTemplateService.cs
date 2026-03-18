using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.TaskGeneration;

internal sealed class TaskTemplateService : ITaskTemplateService
{
    private static readonly IReadOnlyCollection<TaskTemplate> DiscoveryTasks =
    [
        new("Research the neighbourhood", "Understand the area — schools, transport, flood zones, and future development plans — before committing."),
        new("Get mortgage pre-approval", "Know exactly how much you can borrow so you can move quickly when the right property appears."),
        new("Shortlist potential properties", "Narrow your search to properties that fit your budget, location, and must-haves."),
        new("Choose a lawyer or conveyancer", "You'll need a property lawyer to review contracts and handle settlement. Line one up early.")
    ];

    private static readonly IReadOnlyCollection<TaskTemplate> AuctionDiscoveryTasks =
    [
        new("Research the neighbourhood", "Understand the area — schools, transport, flood zones, and future development plans — before committing."),
        new("Get mortgage pre-approval", "Know exactly how much you can borrow so you can move quickly when the right property appears."),
        new("Arrange building report before auction", "Auction purchases are unconditional — get your building report done before bidding day."),
        new("Confirm finance is unconditional", "You cannot add a finance condition at auction. Your lending must be fully approved before you bid."),
        new("Choose a lawyer or conveyancer", "You'll need a property lawyer to review contracts and handle settlement. Line one up early.")
    ];

    private static readonly IReadOnlyCollection<TaskTemplate> AcceptedOfferTasks =
    [
        new("Confirm lawyer details", "Your lawyer needs to be engaged now to review the sale and purchase agreement."),
        new("Review active conditions", "Understand which conditions are active and their due dates so nothing is missed."),
        new("Confirm settlement date", "Make sure the settlement date works for your financing, move-in plans, and lawyer availability.")
    ];

    private static readonly IReadOnlyCollection<TaskTemplate> AuctionAcceptedOfferTasks =
    [
        new("Confirm lawyer details", "Your lawyer needs to be engaged now to review the sale and purchase agreement."),
        new("Confirm auction registration", "Register with the auctioneer and confirm your bidder number before auction day."),
        new("Review auction terms", "Understand the contract terms — auction sales are unconditional once the hammer falls."),
        new("Confirm settlement date", "Make sure the settlement date works for your financing, move-in plans, and lawyer availability.")
    ];

    private static readonly IReadOnlyCollection<TaskTemplate> TenderAcceptedOfferTasks =
    [
        new("Confirm lawyer details", "Your lawyer needs to be engaged now to review the sale and purchase agreement."),
        new("Prepare tender submission", "Complete your tender offer with any conditions, price, and terms before the deadline."),
        new("Confirm tender deadline", "Tenders close at a fixed time — missing the deadline means missing the property."),
        new("Confirm settlement date", "Make sure the settlement date works for your financing, move-in plans, and lawyer availability.")
    ];

    private static readonly IReadOnlyCollection<TaskTemplate> DeadlineAcceptedOfferTasks =
    [
        new("Confirm lawyer details", "Your lawyer needs to be engaged now to review the sale and purchase agreement."),
        new("Review deadline sale terms", "Understand the terms and any multi-offer process that may apply."),
        new("Confirm settlement date", "Make sure the settlement date works for your financing, move-in plans, and lawyer availability.")
    ];

    private static readonly IReadOnlyCollection<TaskTemplate> PreSettlementTasks =
    [
        new("Confirm final loan approval", "Your lender needs to provide final unconditional approval and confirm drawdown on settlement day."),
        new("Confirm insurance active from settlement date", "You become responsible for the property from settlement — insurance must be in place."),
        new("Complete final inspection", "Walk through the property before settlement to confirm its condition matches the agreement."),
        new("Confirm settlement funds ready", "Ensure deposit and balance are available and your lawyer has drawdown instructions."),
        new("Confirm key collection details", "Arrange where and when you'll collect keys after settlement completes.")
    ];

    private static readonly IReadOnlyCollection<TaskTemplate> SettlementDayTasks =
    [
        new("Lawyer confirms settlement", "Your lawyer will confirm that settlement has completed and title has transferred."),
        new("Funds transferred", "The purchase price is transferred from your lender to the vendor's solicitor."),
        new("Keys collected", "Collect the keys — the property is now yours.")
    ];

    public IReadOnlyCollection<TaskTemplate> GetDiscoveryTasks(BuyingMethod buyingMethod)
    {
        return buyingMethod switch
        {
            BuyingMethod.Auction => AuctionDiscoveryTasks,
            _ => DiscoveryTasks
        };
    }

    public IReadOnlyCollection<TaskTemplate> GetAcceptedOfferTasks(BuyingMethod buyingMethod)
    {
        return buyingMethod switch
        {
            BuyingMethod.Auction => AuctionAcceptedOfferTasks,
            BuyingMethod.Tender => TenderAcceptedOfferTasks,
            BuyingMethod.Deadline => DeadlineAcceptedOfferTasks,
            _ => AcceptedOfferTasks
        };
    }

    public IReadOnlyCollection<TaskTemplate> GetConditionTasks(ConditionType conditionType)
    {
        return conditionType switch
        {
            ConditionType.Finance =>
            [
                new("Submit finance documents", "Provide your lender with the signed agreement and any required documentation."),
                new("Confirm lender approval", "Get written confirmation that your finance has been approved before the condition due date.")
            ],
            ConditionType.BuildingReport =>
            [
                new("Book building inspection", "Arrange a qualified inspector to assess the property's structural and weather-tightness condition."),
                new("Review building report", "Review the findings with your lawyer — significant issues may need negotiation or withdrawal.")
            ],
            ConditionType.Lim =>
            [
                new("Obtain LIM report", "Request a Land Information Memorandum from the local council — it reveals consents, hazards, and compliance history."),
                new("Review LIM findings", "Check for non-consented work, natural hazard zones, or council notices that could affect the property.")
            ],
            ConditionType.Insurance =>
            [
                new("Obtain insurance quote", "Get a quote for house and contents insurance to confirm the property is insurable at a reasonable cost."),
                new("Confirm insurance cover", "Accept and activate your insurance policy so cover begins from settlement day.")
            ],
            ConditionType.SolicitorApproval =>
            [
                new("Send agreement to lawyer", "Provide the sale and purchase agreement to your lawyer for review and advice."),
                new("Confirm solicitor approval", "Your lawyer confirms the agreement is satisfactory and you are comfortable proceeding.")
            ],
            _ => []
        };
    }

    public IReadOnlyCollection<TaskTemplate> GetPreSettlementTasks() => PreSettlementTasks;

    public IReadOnlyCollection<TaskTemplate> GetSettlementTasks() => SettlementDayTasks;
}
