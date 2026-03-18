using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Common;

internal static class EnumText
{
    public static string PropertyStatus(PropertyStatus status)
    {
        return status switch
        {
            Domain.Properties.PropertyStatus.Discovery => "discovery",
            Domain.Properties.PropertyStatus.OfferPreparation => "offer_preparation",
            Domain.Properties.PropertyStatus.Submitted => "submitted",
            Domain.Properties.PropertyStatus.Conditional => "conditional",
            Domain.Properties.PropertyStatus.Unconditional => "unconditional",
            Domain.Properties.PropertyStatus.SettlementPending => "settlement_pending",
            Domain.Properties.PropertyStatus.Settled => "settled",
            Domain.Properties.PropertyStatus.Archived => "archived",
            Domain.Properties.PropertyStatus.Cancelled => "cancelled",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string WorkspaceStage(PropertyStatus status)
    {
        return PropertyStatus(status);
    }

    public static string ConditionType(ConditionType type)
    {
        return type switch
        {
            Domain.Properties.ConditionType.Finance => "finance",
            Domain.Properties.ConditionType.BuildingReport => "building_report",
            Domain.Properties.ConditionType.Lim => "lim",
            Domain.Properties.ConditionType.Insurance => "insurance",
            Domain.Properties.ConditionType.SolicitorApproval => "solicitor_approval",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static string ConditionStatus(ConditionStatus status)
    {
        return status switch
        {
            Domain.Properties.ConditionStatus.Pending => "pending",
            Domain.Properties.ConditionStatus.Satisfied => "satisfied",
            Domain.Properties.ConditionStatus.Waived => "waived",
            Domain.Properties.ConditionStatus.Failed => "failed",
            Domain.Properties.ConditionStatus.Expired => "expired",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string TaskStage(TaskStage stage)
    {
        return stage switch
        {
            Domain.Properties.TaskStage.Discovery => "discovery",
            Domain.Properties.TaskStage.OfferPreparation => "offer_preparation",
            Domain.Properties.TaskStage.Submitted => "submitted",
            Domain.Properties.TaskStage.Conditional => "conditional",
            Domain.Properties.TaskStage.Unconditional => "unconditional",
            Domain.Properties.TaskStage.SettlementPending => "settlement_pending",
            Domain.Properties.TaskStage.Settlement => "settlement",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
    }

    public static string TaskStatus(KeyPilot.Domain.Properties.TaskStatus status)
    {
        return status switch
        {
            Domain.Properties.TaskStatus.Pending => "pending",
            Domain.Properties.TaskStatus.Completed => "completed",
            Domain.Properties.TaskStatus.NeedsAttention => "needs_attention",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string TaskImportance(TaskImportance importance)
    {
        return importance switch
        {
            Domain.Properties.TaskImportance.Mandatory => "mandatory",
            Domain.Properties.TaskImportance.Recommended => "recommended",
            Domain.Properties.TaskImportance.Informational => "informational",
            _ => throw new ArgumentOutOfRangeException(nameof(importance), importance, null)
        };
    }

    public static string WorkspaceReminderStatus(WorkspaceReminderStatus status)
    {
        return status switch
        {
            Domain.Properties.WorkspaceReminderStatus.Pending => "pending",
            Domain.Properties.WorkspaceReminderStatus.Sent => "sent",
            Domain.Properties.WorkspaceReminderStatus.Cancelled => "cancelled",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string BuyingMethod(BuyingMethod method)
    {
        return method switch
        {
            Domain.Properties.BuyingMethod.PrivateSale => "private_sale",
            Domain.Properties.BuyingMethod.Auction => "auction",
            Domain.Properties.BuyingMethod.Negotiation => "negotiation",
            Domain.Properties.BuyingMethod.Tender => "tender",
            Domain.Properties.BuyingMethod.Deadline => "deadline",
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
    }
}
