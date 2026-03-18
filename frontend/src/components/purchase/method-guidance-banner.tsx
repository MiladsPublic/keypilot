import { AlertTriangle, Gavel, Info, ShieldCheck } from "lucide-react";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { type BuyingMethod, type Property } from "@/features/properties/types/property";

type MethodGuidanceBannerProps = {
  property: Property;
};

function overdueConditionCount(property: Property) {
  return property.readinessSummary.overdueConditions;
}

function openConditionCount(property: Property) {
  return property.readinessSummary.openConditions;
}

function isSettled(property: Property) {
  return property.status === "settled";
}

function isCancelled(property: Property) {
  return property.status === "cancelled";
}

type GuidanceContent = {
  icon: React.ReactNode;
  title: string;
  description: string;
  className: string;
};

function getGuidance(property: Property): GuidanceContent | null {
  if (isSettled(property) || isCancelled(property)) {
    return null;
  }

  const overdue = overdueConditionCount(property);
  const open = openConditionCount(property);

  if (overdue > 0) {
    return {
      icon: <AlertTriangle className="h-4 w-4" />,
      title: `${overdue} condition${overdue > 1 ? "s" : ""} overdue`,
      description: "Review overdue conditions now — these may block your purchase from going unconditional.",
      className: "border-red-200 bg-red-50 text-red-900 [&>svg]:text-red-600",
    };
  }

  if (property.readinessSummary.overdueTasks > 0) {
    const count = property.readinessSummary.overdueTasks;
    return {
      icon: <AlertTriangle className="h-4 w-4" />,
      title: `${count} task${count > 1 ? "s" : ""} overdue`,
      description: "Complete overdue tasks to keep your purchase on track for settlement.",
      className: "border-amber-200 bg-amber-50 text-amber-900 [&>svg]:text-amber-600",
    };
  }

  return getMethodGuidance(property.buyingMethod, property, open);
}

function getMethodGuidance(method: BuyingMethod, property: Property, openConditions: number): GuidanceContent | null {
  switch (method) {
    case "auction":
      return {
        icon: <Gavel className="h-4 w-4" />,
        title: "Unconditional auction purchase",
        description: property.daysUntilSettlement <= 7
          ? `Settlement is in ${property.daysUntilSettlement} day${property.daysUntilSettlement !== 1 ? "s" : ""}. Confirm your lawyer and funds are ready.`
          : "No conditions apply — focus on completing settlement preparation tasks.",
        className: "border-sky-200 bg-sky-50 text-sky-900 [&>svg]:text-sky-600",
      };

    case "tender":
      if (openConditions > 0) {
        return {
          icon: <ShieldCheck className="h-4 w-4" />,
          title: "Tender accepted — conditions active",
          description: "Tenders often have limited conditions. Confirm solicitor approval promptly to stay on track.",
          className: "border-sky-200 bg-sky-50 text-sky-900 [&>svg]:text-sky-600",
        };
      }
      return null;

    case "deadline":
      if (openConditions > 0) {
        return {
          icon: <ShieldCheck className="h-4 w-4" />,
          title: "Deadline sale accepted — conditions active",
          description: `You have ${openConditions} active condition${openConditions > 1 ? "s" : ""} to complete before going unconditional.`,
          className: "border-sky-200 bg-sky-50 text-sky-900 [&>svg]:text-sky-600",
        };
      }
      return null;

    case "private_sale":
    case "negotiation":
      if (openConditions > 0 && property.status === "conditional") {
        return {
          icon: <Info className="h-4 w-4" />,
          title: `${openConditions} condition${openConditions > 1 ? "s" : ""} remaining`,
          description: "Complete or waive your conditions to move to unconditional status.",
          className: "border-sky-200 bg-sky-50 text-sky-900 [&>svg]:text-sky-600",
        };
      }
      if (property.readinessSummary.isReadyToSettle) {
        return {
          icon: <ShieldCheck className="h-4 w-4" />,
          title: "Ready to settle",
          description: "All conditions cleared and tasks complete. You're ready for settlement day.",
          className: "border-emerald-200 bg-emerald-50 text-emerald-900 [&>svg]:text-emerald-600",
        };
      }
      return null;

    default:
      return null;
  }
}

export function MethodGuidanceBanner({ property }: MethodGuidanceBannerProps) {
  const guidance = getGuidance(property);

  if (!guidance) {
    return null;
  }

  return (
    <Alert className={`rounded-xl ${guidance.className}`}>
      {guidance.icon}
      <AlertTitle>{guidance.title}</AlertTitle>
      <AlertDescription>{guidance.description}</AlertDescription>
    </Alert>
  );
}
