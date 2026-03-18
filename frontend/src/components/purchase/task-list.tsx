import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { type BuyingMethod, type PropertyTask } from "@/features/properties/types/property";
import { TaskItem } from "@/components/purchase/task-item";
import { formatStage } from "@/components/purchase/utils";

type TaskListProps = {
  title?: string;
  groupedTasks: Array<{ stage: string; tasks: PropertyTask[] }>;
  buyingMethod?: BuyingMethod;
  disabled?: boolean;
  onToggleTask?: (task: PropertyTask) => void;
};

export function TaskList({ title = "All tasks", groupedTasks, buyingMethod, disabled, onToggleTask }: TaskListProps) {
  return (
    <Card className="rounded-2xl">
      <CardHeader>
        <CardTitle className="text-lg">{title}</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {groupedTasks.map((entry) => (
          <div key={entry.stage} className="space-y-3">
            <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">{formatStage(entry.stage, buyingMethod)}</p>
            {entry.tasks.length === 0 ? <p className="text-sm text-ink/60">No tasks remaining in this stage.</p> : null}
            {entry.tasks.map((task) => (
              <TaskItem key={task.id} task={task} disabled={disabled} onToggle={onToggleTask} />
            ))}
          </div>
        ))}
      </CardContent>
    </Card>
  );
}
