import { apiClient } from "@/lib/api-client";
import { type Property } from "@/features/properties/types/property";

export async function cancelProperty(id: string, token?: string | null): Promise<Property> {
  return apiClient.patch<Property>(`/api/v1/properties/${id}/cancel`, undefined, token);
}
