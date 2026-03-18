import { apiClient } from "@/lib/api-client";

export async function deleteDocument(id: string, token?: string | null): Promise<void> {
  return apiClient.delete(`/api/v1/documents/${encodeURIComponent(id)}`, token);
}
