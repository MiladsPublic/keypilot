using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Common;

internal static class EnumText
{
    public static string PropertyStatus(PropertyStatus status)
    {
        return status switch
        {
            Domain.Properties.PropertyStatus.AcceptedOffer => "accepted_offer",
            Domain.Properties.PropertyStatus.Conditional => "conditional",
            Domain.Properties.PropertyStatus.Unconditional => "unconditional",
            Domain.Properties.PropertyStatus.PreSettlement => "pre_settlement",
            Domain.Properties.PropertyStatus.Settled => "settled",
            Domain.Properties.PropertyStatus.Cancelled => "cancelled",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
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
            Domain.Properties.TaskStage.AcceptedOffer => "accepted_offer",
            Domain.Properties.TaskStage.Conditional => "conditional",
            Domain.Properties.TaskStage.Unconditional => "unconditional",
            Domain.Properties.TaskStage.PreSettlement => "pre_settlement",
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
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
