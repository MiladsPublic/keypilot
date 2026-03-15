export interface Condition {
  id: string;
  type: "finance" | "building_report" | "lim" | "insurance" | "solicitor_approval";
  dueDate: string;
  status: "pending" | "completed" | "expired";
  completedAtUtc: string | null;
}

export interface PropertyTask {
  id: string;
  conditionId: string | null;
  title: string;
  stage: "accepted_offer" | "conditional" | "unconditional" | "pre_settlement" | "settlement";
  dueDate: string | null;
  status: "pending" | "completed";
  completedAtUtc: string | null;
}

export interface TaskSummary {
  completed: number;
  total: number;
  pending: number;
}

export interface Property {
  id: string;
  workspaceId: string | null;
  address: string;
  status: "accepted_offer" | "conditional" | "unconditional" | "pre_settlement" | "settled";
  acceptedOfferDate: string;
  settlementDate: string;
  daysUntilSettlement: number;
  purchasePrice: number | null;
  depositAmount: number | null;
  conditions: Condition[];
  tasks: PropertyTask[];
  taskSummary: TaskSummary;
  createdAtUtc: string;
}
