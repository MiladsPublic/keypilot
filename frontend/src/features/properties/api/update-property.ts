import { apiClient } from "@/lib/api-client";
import { type Property } from "@/features/properties/types/property";

export type UpdatePropertyBody = {
  address?: string;
  acceptedOfferDate?: string | null;
  settlementDate?: string | null;
  purchasePrice?: number | null;
  depositAmount?: number | null;
  methodReference?: string | null;
};

export async function updateProperty(
  id: string,
  body: UpdatePropertyBody,
  token?: string | null
): Promise<Property> {
  return apiClient.patch<Property, UpdatePropertyBody>(`/api/v1/properties/${id}`, body, token);
}
