import { apiClient } from "@/lib/api-client";
import { type CreatePropertyFormValues } from "@/features/properties/schemas/create-property-schema";
import { type Property } from "@/features/properties/types/property";

export async function createProperty(values: CreatePropertyFormValues): Promise<Property> {
  return apiClient.post<Property, Record<string, string | number | null>>("/api/v1/properties", {
    address: values.address.trim(),
    offerAcceptedDate: values.offerAcceptedDate || null,
    settlementDate: values.settlementDate || null,
    purchasePrice: values.purchasePrice ? Number(values.purchasePrice) : null
  });
}
