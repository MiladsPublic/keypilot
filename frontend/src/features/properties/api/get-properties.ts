import { apiClient } from "@/lib/api-client";
import { type Property } from "@/features/properties/types/property";

export async function getProperties(token?: string | null): Promise<Property[]> {
  return apiClient.get<Property[]>("/api/v1/properties", token);
}
