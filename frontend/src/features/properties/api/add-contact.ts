import { apiClient } from "@/lib/api-client";
import { type Contact } from "@/features/properties/types/property";

export interface AddContactBody {
  role: string;
  name: string;
  email?: string | null;
  phone?: string | null;
}

export async function addContact(
  propertyId: string,
  body: AddContactBody,
  token?: string | null
): Promise<Contact> {
  return apiClient.post<Contact, AddContactBody>(
    `/api/v1/properties/${encodeURIComponent(propertyId)}/contacts`,
    body,
    token
  );
}
