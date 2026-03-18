import { Home, MapPin } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { type Property } from "@/features/properties/types/property";
import { badgeVariantForStatus, formatStage, formatDate } from "@/components/purchase/utils";
import { cn } from "@/lib/utils";

type PropertyListCardProps = {
  properties: Property[];
  selectedPropertyId: string;
  onSelectProperty: (id: string) => void;
};

function urgencyScore(property: Property): number {
  const overdue = property.readinessSummary.overdueConditions + property.readinessSummary.overdueTasks;
  if (overdue > 0) return 2;
  if (property.daysUntilSettlement != null && property.daysUntilSettlement <= 7) return 1;
  return 0;
}

export function PropertyListCard({ properties, selectedPropertyId, onSelectProperty }: PropertyListCardProps) {
  const sorted = [...properties].sort((a, b) => urgencyScore(b) - urgencyScore(a));

  return (
    <Card className="rounded-2xl">
      <CardHeader>
        <div className="flex items-center gap-2">
          <Home className="h-4 w-4 text-ink/70" />
          <CardTitle className="text-lg">Your properties</CardTitle>
        </div>
      </CardHeader>
      <CardContent>
        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
          {sorted.map((property) => {
            const isSelected = property.id === selectedPropertyId;
            const overdue = property.readinessSummary.overdueConditions + property.readinessSummary.overdueTasks;
            const addressParts = property.address.split(",").map((p) => p.trim()).filter(Boolean);
            const headline = addressParts[0] ?? property.address;
            const sub = addressParts.slice(1).join(", ");
            const nextDate = property.readinessSummary.nextCriticalDate;
            const nextLabel = property.readinessSummary.nextCriticalDateLabel;

            return (
              <button
                key={property.id}
                type="button"
                onClick={() => onSelectProperty(property.id)}
                className={cn(
                  "rounded-xl border p-4 text-left transition-colors",
                  isSelected
                    ? "border-ink/30 bg-ink/[0.04]"
                    : "border-line hover:border-ink/20 hover:bg-ink/[0.02]"
                )}
              >
                <p className="truncate text-sm font-medium">{headline}</p>
                {sub ? (
                  <p className="mt-0.5 flex items-center gap-1 truncate text-xs text-ink/55">
                    <MapPin className="h-3 w-3 shrink-0" />
                    {sub}
                  </p>
                ) : null}
                <div className="mt-2 flex flex-wrap items-center gap-1.5">
                  <Badge variant={badgeVariantForStatus(property.status)} className="text-xs">
                    {formatStage(property.status, property.buyingMethod)}
                  </Badge>
                  {overdue > 0 ? (
                    <Badge variant="danger" className="text-xs">
                      {overdue} overdue
                    </Badge>
                  ) : null}
                </div>
                {nextDate && nextLabel ? (
                  <p className="mt-1.5 truncate text-xs text-ink/55">
                    {nextLabel} · {formatDate(nextDate)}
                  </p>
                ) : null}
              </button>
            );
          })}
        </div>
      </CardContent>
    </Card>
  );
}
