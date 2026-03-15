import { CalendarClock, MapPinHouse, Wallet } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { type Property } from "@/features/properties/types/property";

function formatDate(value: string | null) {
  if (!value) {
    return "Not set";
  }

  const normalizedValue = value.includes("T") ? value : `${value}T00:00:00`;

  return new Intl.DateTimeFormat("en-NZ", {
    day: "numeric",
    month: "short",
    year: "numeric"
  }).format(new Date(normalizedValue));
}

function formatCurrency(value: number | null) {
  if (value === null) {
    return "Not set";
  }

  return new Intl.NumberFormat("en-NZ", {
    style: "currency",
    currency: "NZD",
    maximumFractionDigits: 0
  }).format(value);
}

export function PropertySummaryCard({ property }: { property: Property }) {
  return (
    <Card>
      <CardHeader>
        <div className="flex items-start justify-between gap-4">
          <div className="space-y-2">
            <Badge>{property.status}</Badge>
            <CardTitle className="text-2xl">{property.address}</CardTitle>
          </div>
          <div className="rounded-full border border-line bg-canvas px-3 py-2 text-xs font-medium text-ink/70">
            Created {formatDate(property.createdAtUtc)}
          </div>
        </div>
      </CardHeader>
      <CardContent className="grid gap-4 md:grid-cols-3">
        <div className="rounded-3xl bg-canvas p-4">
          <MapPinHouse className="mb-3 h-5 w-5 text-accent" />
          <p className="text-xs uppercase tracking-[0.18em] text-ink/55">Address</p>
          <p className="mt-2 text-sm font-semibold">{property.address}</p>
        </div>
        <div className="rounded-3xl bg-canvas p-4">
          <CalendarClock className="mb-3 h-5 w-5 text-accent" />
          <p className="text-xs uppercase tracking-[0.18em] text-ink/55">Timeline</p>
          <p className="mt-2 text-sm font-semibold">Offer accepted: {formatDate(property.offerAcceptedDate)}</p>
          <p className="mt-1 text-sm font-semibold">Settlement: {formatDate(property.settlementDate)}</p>
        </div>
        <div className="rounded-3xl bg-canvas p-4">
          <Wallet className="mb-3 h-5 w-5 text-accent" />
          <p className="text-xs uppercase tracking-[0.18em] text-ink/55">Purchase price</p>
          <p className="mt-2 text-sm font-semibold">{formatCurrency(property.purchasePrice)}</p>
        </div>
      </CardContent>
    </Card>
  );
}
