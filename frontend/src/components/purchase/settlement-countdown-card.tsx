import { CalendarClock } from "lucide-react";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { formatDate, countdownTone } from "@/components/purchase/utils";

export function SettlementCountdownCard({ settlementDate, daysUntilSettlement }: { settlementDate: string; daysUntilSettlement: number }) {
  const tone = countdownTone(daysUntilSettlement);

  return (
    <Card className="rounded-2xl">
      <CardHeader>
        <CardTitle className="text-lg">Settlement</CardTitle>
      </CardHeader>
      <CardContent className="space-y-2">
        <p
          className={
            tone === "danger"
              ? "text-2xl font-semibold text-[var(--danger-fg)]"
              : tone === "warning"
                ? "text-2xl font-semibold text-[var(--warning-fg)]"
                : "text-2xl font-semibold"
          }
        >
          {daysUntilSettlement <= 0 ? "Settlement day" : `Settlement in ${daysUntilSettlement} days`}
        </p>
        <p className="inline-flex items-center gap-1 text-sm text-ink/65">
          <CalendarClock className="h-4 w-4" />
          {formatDate(settlementDate)}
        </p>
      </CardContent>
    </Card>
  );
}
