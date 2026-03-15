using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.TaskGeneration;

internal sealed class TaskTemplateService : ITaskTemplateService
{
    private static readonly IReadOnlyCollection<string> AcceptedOfferTasks =
    [
        "Confirm lawyer details",
        "Review active conditions",
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

    private static readonly IReadOnlyCollection<string> SettlementTasks =
    [
        "Lawyer confirms settlement",
        "Funds transferred",
        "Keys collected"
    ];

    public IReadOnlyCollection<string> GetAcceptedOfferTasks() => AcceptedOfferTasks;

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

    public IReadOnlyCollection<string> GetSettlementTasks() => SettlementTasks;
}
