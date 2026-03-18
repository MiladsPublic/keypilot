import { apiClient } from "@/lib/api-client";
import { type Document } from "@/features/properties/types/property";

export interface AddDocumentBody {
  storageKey: string;
  fileName: string;
  category: string;
}

export async function addDocument(
  propertyId: string,
  body: AddDocumentBody,
  token?: string | null
): Promise<Document> {
  return apiClient.post<Document, AddDocumentBody>(
    `/api/v1/properties/${encodeURIComponent(propertyId)}/documents`,
    body,
    token
  );
}
