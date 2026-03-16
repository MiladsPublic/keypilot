import { CheckCircle2 } from "lucide-react";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";

export function ProgressCard({
  completed,
  total,
  pending,
  openConditions
}: {
  completed: number;
  total: number;
  pending: number;
  openConditions: number;
}) {
  const progressValue = total === 0 ? 0 : Math.round((completed / total) * 100);

  return (
    <Card className="rounded-2xl">
      <CardHeader>
        <div className="flex items-center gap-2">
          <CheckCircle2 className="h-4 w-4 text-ink/70" />
          <CardTitle className="text-lg">Progress</CardTitle>
        </div>
      </CardHeader>
      <CardContent className="space-y-3">
        <p className="text-sm font-medium">
          {completed} / {total} tasks completed
        </p>
        <Progress value={progressValue} />
        <p className="text-sm text-ink/65">{pending} tasks pending</p>
        <p className="text-sm text-ink/65">{openConditions} conditions still open</p>
      </CardContent>
    </Card>
  );
}
