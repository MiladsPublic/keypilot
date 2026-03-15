import { apiClient } from "@/lib/api-client";
import { type Property } from "@/features/properties/types/property";

export async function settleProperty(id: string, token?: string | null): Promise<Property> {
  return apiClient.patch<Property>(`/api/v1/properties/${id}/settle`, undefined, token);
}
