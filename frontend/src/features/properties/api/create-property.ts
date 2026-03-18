import { apiClient } from "@/lib/api-client";
import { type CreatePropertyFormValues } from "@/features/properties/schemas/create-property-schema";
import { type Property } from "@/features/properties/types/property";

type CreatePropertyRequest = {
  address: string;
  buyingMethod: string;
  acceptedOfferDate: string | null;
  settlementDate: string | null;
  purchasePrice: number | null;
  depositAmount: number | null;
  methodReference: string | null;
  conditions: Array<{
    type: string;
    daysFromAcceptedOffer: number;
  }>;
};

const fallbackOffsets: Record<string, number> = {
  finance: 5,
  building_report: 5,
  lim: 10,
  insurance: 10,
  solicitor_approval: 5
};

export async function createProperty(
  values: CreatePropertyFormValues,
  conditionOffsets?: Record<string, number>,
  token?: string | null
): Promise<Property> {
  const offsets = conditionOffsets ?? fallbackOffsets;
  const conditions = Object.entries(values.conditions)
    .filter(([, selected]) => selected)
    .map(([type]) => ({
      type,
      daysFromAcceptedOffer: offsets[type] ?? 5
    }));

  return apiClient.post<Property, CreatePropertyRequest>("/api/v1/properties", {
    address: values.address.trim(),
    buyingMethod: values.buyingMethod,
    acceptedOfferDate: values.acceptedOfferDate || null,
    settlementDate: values.settlementDate || null,
    purchasePrice: values.purchasePrice ? Number(values.purchasePrice) : null,
    depositAmount: values.depositAmount ? Number(values.depositAmount) : null,
    methodReference: values.methodReference?.trim() || null,
    conditions
  }, token);
}
