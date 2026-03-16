import { ListChecks } from "lucide-react";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { type PropertyTask } from "@/features/properties/types/property";
import { TaskItem } from "@/components/purchase/task-item";

export function NextActionsCard({
  tasks,
  disabled,
  onToggleTask
}: {
  tasks: PropertyTask[];
  disabled?: boolean;
  onToggleTask: (task: PropertyTask) => void;
}) {
  return (
    <Card className="rounded-2xl">
      <CardHeader>
        <div className="flex items-center gap-2">
          <ListChecks className="h-4 w-4 text-ink/70" />
          <CardTitle className="text-lg">Next actions</CardTitle>
        </div>
      </CardHeader>
      <CardContent className="space-y-3">
        {tasks.length === 0 ? <p className="text-sm text-ink/60">No tasks remaining in this stage.</p> : null}
        {tasks.map((task) => (
          <TaskItem key={task.id} task={task} disabled={disabled} onToggle={onToggleTask} />
        ))}
      </CardContent>
    </Card>
  );
}
