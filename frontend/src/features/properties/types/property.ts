export interface Condition {
  id: string;
  type: "finance" | "building_report" | "lim" | "insurance" | "solicitor_approval";
  dueDate: string;
  status: "pending" | "satisfied" | "waived" | "failed" | "expired";
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

export interface PurchaseReadinessSummary {
  mode: "conditional" | "settlement";
  blockingConditions: number;
  openConditions: number;
  overdueConditions: number;
  pendingTasks: number;
  overdueTasks: number;
  settlementTasksRemaining: number;
  isReadyToSettle: boolean;
  nextAction: string | null;
}

export type BuyingMethod = "private_sale" | "auction" | "negotiation" | "tender" | "deadline";

export interface Document {
  id: string;
  propertyId: string;
  storageKey: string;
  fileName: string;
  category: string;
  createdAtUtc: string;
}

export interface Contact {
  id: string;
  propertyId: string;
  role: string;
  name: string;
  email: string | null;
  phone: string | null;
  createdAtUtc: string;
}

export interface Property {
  id: string;
  workspaceId: string | null;
  address: string;
  buyingMethod: BuyingMethod;
  status: "accepted_offer" | "conditional" | "unconditional" | "pre_settlement" | "settled" | "cancelled";
  acceptedOfferDate: string;
  unconditionalDate: string | null;
  settlementDate: string;
  settledDate: string | null;
  cancelledDate: string | null;
  daysUntilSettlement: number;
  purchasePrice: number | null;
  depositAmount: number | null;
  conditions: Condition[];
  tasks: PropertyTask[];
  documents: Document[];
  contacts: Contact[];
  taskSummary: TaskSummary;
  readinessSummary: PurchaseReadinessSummary;
  createdAtUtc: string;
}
