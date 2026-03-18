import { AlertTriangle, CalendarClock } from "lucide-react";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { formatDate } from "@/components/purchase/utils";
import { type PurchaseReadinessSummary } from "@/features/properties/types/property";

export function NextCriticalDateCard({ readinessSummary }: { readinessSummary: PurchaseReadinessSummary }) {
  if (!readinessSummary.nextCriticalDate || !readinessSummary.nextCriticalDateLabel) {
    return null;
  }

  const dateStr = readinessSummary.nextCriticalDate;
  const dueDate = new Date(`${dateStr}T00:00:00`).getTime();
  const today = new Date();
  const now = new Date(today.getFullYear(), today.getMonth(), today.getDate()).getTime();
  const diffDays = Math.floor((dueDate - now) / (1000 * 60 * 60 * 24));

  const isUrgent = diffDays <= 3;
  const isWarning = diffDays <= 7 && diffDays > 3;

  return (
    <Card className="rounded-2xl">
      <CardHeader>
        <div className="flex items-center gap-2">
          {isUrgent ? (
            <AlertTriangle className="h-4 w-4 text-[var(--danger-fg)]" />
          ) : (
            <CalendarClock className="h-4 w-4 text-ink/70" />
          )}
          <CardTitle className="text-lg">Next deadline</CardTitle>
        </div>
      </CardHeader>
      <CardContent className="space-y-1">
        <p
          className={
            isUrgent
              ? "text-lg font-semibold text-[var(--danger-fg)]"
              : isWarning
                ? "text-lg font-semibold text-[var(--warning-fg)]"
                : "text-lg font-semibold"
          }
        >
          {readinessSummary.nextCriticalDateLabel}
        </p>
        <p className="text-sm text-ink/65">
          {formatDate(dateStr)}
          {diffDays === 0 ? " — today" : diffDays === 1 ? " — tomorrow" : diffDays > 0 ? ` — ${diffDays} days away` : ` — ${Math.abs(diffDays)} days overdue`}
        </p>
      </CardContent>
    </Card>
  );
}
