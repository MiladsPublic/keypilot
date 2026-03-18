import { Bell } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { type WorkspaceReminder } from "@/features/properties/types/property";
import { formatDate } from "@/components/purchase/utils";

export function RemindersCard({ reminders }: { reminders: WorkspaceReminder[] }) {
  const pending = reminders.filter((r) => r.status === "pending");
  const sent = reminders.filter((r) => r.status === "sent");

  if (reminders.length === 0) {
    return null;
  }

  return (
    <Card className="rounded-2xl">
      <CardHeader>
        <div className="flex items-center gap-2">
          <Bell className="h-4 w-4 text-ink/70" />
          <CardTitle className="text-lg">Reminders</CardTitle>
        </div>
      </CardHeader>
      <CardContent className="space-y-3">
        {pending.length > 0 ? (
          <div className="space-y-2">
            {pending.map((reminder) => (
              <div key={reminder.id} className="flex items-center justify-between text-sm">
                <span>{reminder.title}</span>
                <Badge variant="secondary">{formatDate(reminder.scheduledForUtc)}</Badge>
              </div>
            ))}
          </div>
        ) : null}
        {sent.length > 0 ? (
          <p className="text-sm text-ink/65">{sent.length} reminder{sent.length !== 1 ? "s" : ""} sent</p>
        ) : null}
        {pending.length === 0 && sent.length === 0 ? (
          <p className="text-sm text-ink/65">No active reminders.</p>
        ) : null}
      </CardContent>
    </Card>
  );
}
