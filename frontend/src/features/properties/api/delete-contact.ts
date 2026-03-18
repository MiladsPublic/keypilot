import { apiClient } from "@/lib/api-client";

export async function deleteContact(id: string, token?: string | null): Promise<void> {
  return apiClient.delete(`/api/v1/contacts/${encodeURIComponent(id)}`, token);
}
