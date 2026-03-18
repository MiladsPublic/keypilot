import { apiClient } from "@/lib/api-client";

export interface ConditionDefault {
  type: string;
  label: string;
  daysFromAcceptedOffer: number;
}

export interface BuyingMethodOption {
  value: string;
  label: string;
}

export interface MethodProfile {
  description: string;
  stages: string[];
  stageLabels: Record<string, string>;
  typicalSettlementDays: number;
}

export interface WorkspaceConfig {
  conditionDefaults: ConditionDefault[];
  buyingMethods: BuyingMethodOption[];
  conditionsByMethod: Record<string, ConditionDefault[]>;
  methodProfiles: Record<string, MethodProfile>;
}

export async function getWorkspaceConfig(): Promise<WorkspaceConfig> {
  return apiClient.get<WorkspaceConfig>("/api/v1/config/workspace");
}
