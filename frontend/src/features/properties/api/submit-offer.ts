import { apiClient } from "@/lib/api-client";
import { type Property } from "@/features/properties/types/property";

export type SubmitOfferBody = {
  acceptedOfferDate: string;
  settlementDate: string;
  conditions?: Array<{
    type: string;
    daysFromAcceptedOffer: number;
  }>;
};

export async function submitOffer(
  id: string,
  body: SubmitOfferBody,
  token?: string | null
): Promise<Property> {
  return apiClient.post<Property, SubmitOfferBody>(`/api/v1/properties/${id}/submit-offer`, body, token);
}
