import { apiClient } from "@/lib/api-client";
import { type Property } from "@/features/properties/types/property";

export async function getProperty(id: string, token?: string | null): Promise<Property> {
  return apiClient.get<Property>(`/api/v1/properties/${id}`, token);
}
