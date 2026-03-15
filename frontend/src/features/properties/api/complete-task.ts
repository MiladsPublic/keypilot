import { apiClient } from "@/lib/api-client";
import { type PropertyTask } from "@/features/properties/types/property";

export async function completeTask(id: string): Promise<PropertyTask> {
  return apiClient.patch<PropertyTask>(`/api/v1/tasks/${id}/complete`);
}
