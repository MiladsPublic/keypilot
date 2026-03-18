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
  description: string | null;
  importance: "mandatory" | "recommended" | "informational";
  notes: string | null;
  stage: "discovery" | "offer_preparation" | "submitted" | "conditional" | "unconditional" | "settlement_pending" | "settlement";
  dueDate: string | null;
  status: "pending" | "completed" | "needs_attention";
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

export interface WorkspaceReminder {
  id: string;
  taskId: string | null;
  key: string;
  title: string;
  scheduledForUtc: string;
  status: "pending" | "sent" | "cancelled";
  sentAtUtc: string | null;
  cancelledAtUtc: string | null;
}

export interface Property {
  id: string;
  workspaceId: string | null;
  address: string;
  buyingMethod: BuyingMethod;
  methodReference: string | null;
  status: "discovery" | "offer_preparation" | "submitted" | "conditional" | "unconditional" | "settlement_pending" | "settled" | "archived" | "cancelled";
  acceptedOfferDate: string | null;
  unconditionalDate: string | null;
  settlementDate: string | null;
  settledDate: string | null;
  cancelledDate: string | null;
  daysUntilSettlement: number | null;
  purchasePrice: number | null;
  depositAmount: number | null;
  reminders: WorkspaceReminder[];
  conditions: Condition[];
  tasks: PropertyTask[];
  documents: Document[];
  contacts: Contact[];
  taskSummary: TaskSummary;
  readinessSummary: PurchaseReadinessSummary;
  createdAtUtc: string;
}
