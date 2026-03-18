import { CalendarClock } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { type PropertyTask } from "@/features/properties/types/property";
import { badgeVariantForStatus, formatDueLabel } from "@/components/purchase/utils";
import { cn } from "@/lib/utils";

type TaskItemProps = {
  task: PropertyTask;
  disabled?: boolean;
  onToggle?: (task: PropertyTask) => void;
};

export function TaskItem({ task, disabled, onToggle }: TaskItemProps) {
  const dueLabel = formatDueLabel(task.dueDate);
  const isOverdue = dueLabel.startsWith("Overdue");
  const isDueSoon = dueLabel.startsWith("Due today") || dueLabel.startsWith("Due tomorrow") || dueLabel.includes("Due in");

  return (
    <Card className="rounded-xl border border-line shadow-none">
      <CardContent className="flex items-start gap-3 p-4">
        <Checkbox
          className="mt-0.5"
          checked={task.status === "completed"}
          disabled={task.status === "completed" || task.status === "needs_attention" || disabled}
          onCheckedChange={() => onToggle?.(task)}
        />
        <div className="min-w-0 flex-1">
          <p className={cn("text-sm font-medium", task.status === "completed" && "line-through opacity-65")}>{task.title}</p>
          {task.description ? (
            <p className={cn("mt-0.5 text-xs text-ink/55", task.status === "completed" && "line-through opacity-65")}>{task.description}</p>
          ) : null}
          <p
            className={cn(
              "mt-1 inline-flex items-center gap-1 text-sm text-ink/65",
              isOverdue && "text-[var(--danger-fg)]",
              !isOverdue && isDueSoon && "text-[var(--warning-fg)]"
            )}
          >
            <CalendarClock className="h-3.5 w-3.5" />
            {dueLabel}
          </p>
        </div>
        <Badge variant={badgeVariantForStatus(task.status)}>{task.status === "needs_attention" ? "needs attention" : task.status}</Badge>
      </CardContent>
    </Card>
  );
}
