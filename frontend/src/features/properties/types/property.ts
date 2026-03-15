export interface Property {
  id: string;
  workspaceId: string | null;
  address: string;
  status: "draft" | "conditional" | "unconditional" | "settled";
  offerAcceptedDate: string | null;
  settlementDate: string | null;
  purchasePrice: number | null;
  createdAtUtc: string;
}
