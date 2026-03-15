import { apiClient } from "@/lib/api-client";
import { type CreatePropertyFormValues } from "@/features/properties/schemas/create-property-schema";
import { type Property } from "@/features/properties/types/property";

type CreatePropertyRequest = {
  address: string;
  acceptedOfferDate: string;
  settlementDate: string;
  purchasePrice: number | null;
  depositAmount: number | null;
  conditions: Array<{
    type: string;
    daysFromAcceptedOffer: number;
  }>;
};

const conditionOffsets: Record<string, number> = {
  finance: 5,
  building_report: 5,
  lim: 10,
  insurance: 10,
  solicitor_approval: 5
};

export async function createProperty(values: CreatePropertyFormValues, token?: string | null): Promise<Property> {
  const conditions = Object.entries(values.conditions)
    .filter(([, selected]) => selected)
    .map(([type]) => ({
      type,
      daysFromAcceptedOffer: conditionOffsets[type]
    }));

  return apiClient.post<Property, CreatePropertyRequest>("/api/v1/properties", {
    address: values.address.trim(),
    acceptedOfferDate: values.acceptedOfferDate,
    settlementDate: values.settlementDate,
    purchasePrice: values.purchasePrice ? Number(values.purchasePrice) : null,
    depositAmount: values.depositAmount ? Number(values.depositAmount) : null,
    conditions
  }, token);
}
