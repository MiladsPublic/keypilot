import { apiClient } from "@/lib/api-client";
import { type Condition } from "@/features/properties/types/property";

export async function completeCondition(id: string, token?: string | null): Promise<Condition> {
  return apiClient.patch<Condition>(`/api/v1/conditions/${id}/satisfy`, undefined, token);
}
