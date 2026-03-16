import Link from "next/link";
import { Home, MapPin } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import { type Property } from "@/features/properties/types/property";
import { badgeVariantForStatus, formatStage } from "@/components/purchase/utils";

type PurchaseHeroCardProps = {
  property: Property;
  progressValue: number;
  canSelect?: boolean;
  properties?: Property[];
  onSelectProperty?: (id: string) => void;
};

export function PurchaseHeroCard({ property, progressValue, canSelect, properties, onSelectProperty }: PurchaseHeroCardProps) {
  const addressParts = property.address.split(",").map((part) => part.trim()).filter(Boolean);
  const headlineAddress = addressParts[0] ?? property.address;
  const subAddress = addressParts.slice(1).join(", ");

  return (
    <Card className="rounded-2xl">
      <CardContent className="space-y-5 p-6">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div className="space-y-2">
            <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">Current purchase</p>
            <h1 className="text-3xl font-semibold leading-tight md:text-4xl">{headlineAddress}</h1>
            {subAddress ? (
              <p className="inline-flex items-center gap-1 text-sm text-ink/70">
                <MapPin className="h-4 w-4" />
                {subAddress}
              </p>
            ) : null}
          </div>
          <div className="flex items-center gap-2">
            {canSelect && properties ? (
              <select
                value={property.id}
                onChange={(event) => onSelectProperty?.(event.target.value)}
                className="h-10 rounded-xl border border-line bg-white px-3 text-sm"
              >
                {properties.map((item) => (
                  <option key={item.id} value={item.id}>
                    {item.address}
                  </option>
                ))}
              </select>
            ) : null}
            <Button asChild className="rounded-full">
              <Link href="/properties/new">
                <Home className="h-4 w-4" />
                Start a purchase
              </Link>
            </Button>
          </div>
        </div>

        <div className="flex flex-wrap items-center gap-2">
          <Badge variant={badgeVariantForStatus(property.status)}>{formatStage(property.status)}</Badge>
          <p className="text-sm text-ink/70">Purchase stage</p>
        </div>

        <div className="space-y-2">
          <div className="flex items-center justify-between text-sm">
            <p className="text-ink/70">Progress</p>
            <p>{progressValue}%</p>
          </div>
          <Progress value={progressValue} />
        </div>
      </CardContent>
    </Card>
  );
}
