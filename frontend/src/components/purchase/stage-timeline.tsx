import { CheckCircle2, Circle } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { timelineStages, formatStage } from "@/components/purchase/utils";
import { type Property } from "@/features/properties/types/property";

export function StageTimeline({ currentStatus }: { currentStatus: Property["status"] }) {
  const timelineStatus = currentStatus === "pre_settlement" ? "unconditional" : currentStatus;
  const currentStageIndex = timelineStages.indexOf(timelineStatus);

  return (
    <div className="grid gap-3 md:grid-cols-5">
      {timelineStages.map((stage, index) => {
        const isComplete = index < currentStageIndex;
        const isCurrent = index === currentStageIndex;

        return (
          <Card key={stage} className="rounded-2xl shadow-none">
            <CardContent className="p-4">
              <div className="flex items-center gap-2">
                {isComplete ? <CheckCircle2 className="h-4 w-4 text-[var(--success-fg)]" /> : <Circle className="h-4 w-4 text-ink/45" />}
                <p className="text-sm font-medium">{formatStage(stage)}</p>
              </div>
              <Badge variant={isComplete ? "success" : isCurrent ? "default" : "secondary"} className="mt-2">
                {isComplete ? "Completed" : isCurrent ? "Current" : "Upcoming"}
              </Badge>
            </CardContent>
          </Card>
        );
      })}
    </div>
  );
}
