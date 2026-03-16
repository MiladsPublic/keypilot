"use client";

import { useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@clerk/nextjs";
import { useMutation } from "@tanstack/react-query";

import { completeCondition } from "@/features/properties/api/complete-condition";
import { completeTask } from "@/features/properties/api/complete-task";
import { type Condition, type Property, type PropertyTask } from "@/features/properties/types/property";
import { ConditionsCard } from "@/components/purchase/conditions-card";
import { NextActionsCard } from "@/components/purchase/next-actions-card";
import { ProgressCard } from "@/components/purchase/progress-card";
import { PurchaseHeroCard } from "@/components/purchase/purchase-hero-card";
import { SettlementCountdownCard } from "@/components/purchase/settlement-countdown-card";
import { StageTimeline } from "@/components/purchase/stage-timeline";
import { TaskList } from "@/components/purchase/task-list";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { toast } from "@/hooks/use-toast";

type TaskFilter = "all" | "next" | "completed";

function parseDate(value: string | null) {
  if (!value) {
    return Number.POSITIVE_INFINITY;
  }

  const normalizedValue = value.includes("T") ? value : `${value}T00:00:00`;
  return new Date(normalizedValue).getTime();
}

function sortTasks(tasks: PropertyTask[]) {
  return [...tasks].sort((a, b) => {
    if (a.status !== b.status) {
      return a.status === "pending" ? -1 : 1;
    }

    return parseDate(a.dueDate) - parseDate(b.dueDate);
  });
}

function updateTaskSummary(tasks: PropertyTask[]) {
  const total = tasks.length;
  const completed = tasks.filter((task) => task.status === "completed").length;

  return {
    completed,
    total,
    pending: total - completed
  };
}

function groupedTasks(tasks: PropertyTask[]) {
  const groups = ["conditional", "unconditional", "pre_settlement", "settlement"] as const;

  return groups.map((stage) => ({
    stage,
    tasks: tasks.filter((task) => task.stage === stage)
  }));
}

export function PurchaseDashboard({ initialProperties }: { initialProperties: Property[] }) {
  const router = useRouter();
  const { getToken } = useAuth();

  const [properties, setProperties] = useState(initialProperties);
  const [selectedPropertyId, setSelectedPropertyId] = useState(initialProperties[0]?.id ?? "");
  const [taskFilter, setTaskFilter] = useState<TaskFilter>("next");

  const selectedProperty = useMemo(
    () => properties.find((property) => property.id === selectedPropertyId) ?? properties[0],
    [properties, selectedPropertyId]
  );

  const completeTaskMutation = useMutation({
    mutationFn: async (task: PropertyTask) => completeTask(task.id, await getToken()),
    onSuccess: (_data, task) => {
      toast({
        title: "Task completed",
        description: `Marked \"${task.title}\" as complete.`,
        variant: "success"
      });
      router.refresh();
    },
    onError: (_error, task) => {
      toast({
        title: "Couldn't update task",
        description: `Couldn't mark \"${task.title}\" as complete.`,
        variant: "danger"
      });
      router.refresh();
    }
  });

  const completeConditionMutation = useMutation({
    mutationFn: async (condition: Condition) => completeCondition(condition.id, await getToken()),
    onSuccess: (_data, condition) => {
      toast({
        title: "Condition satisfied",
        description: `Marked "${condition.type}" as satisfied.`,
        variant: "success"
      });
      router.refresh();
    },
    onError: (_error, condition) => {
      toast({
        title: "Couldn't update condition",
        description: `Couldn't mark "${condition.type}" as satisfied.`,
        variant: "danger"
      });
      router.refresh();
    }
  });

  if (!selectedProperty) {
    return null;
  }

  const allTasks = sortTasks(selectedProperty.tasks);
  const pendingTasks = allTasks.filter((task) => task.status === "pending");
  const completedTasks = allTasks.filter((task) => task.status === "completed");

  const dueSoonTasks = pendingTasks.filter((task) => {
    if (!task.dueDate) {
      return false;
    }

    const dueDate = new Date(`${task.dueDate}T00:00:00`).getTime();
    const today = new Date();
    const now = new Date(today.getFullYear(), today.getMonth(), today.getDate()).getTime();
    const diffDays = Math.floor((dueDate - now) / (1000 * 60 * 60 * 24));

    return diffDays <= 7;
  });

  const nextTasks = (dueSoonTasks.length > 0 ? dueSoonTasks : pendingTasks).slice(0, 5);

  const filteredTasks =
    taskFilter === "all" ? allTasks : taskFilter === "completed" ? completedTasks : nextTasks;

  const progressValue =
    selectedProperty.taskSummary.total === 0
      ? 0
      : Math.round((selectedProperty.taskSummary.completed / selectedProperty.taskSummary.total) * 100);

  const markTaskComplete = (task: PropertyTask) => {
    if (task.status === "completed") {
      return;
    }

    setProperties((prev) =>
      prev.map((property) => {
        if (property.id !== selectedProperty.id) {
          return property;
        }

        const nextTasksState = property.tasks.map((item) =>
          item.id === task.id
            ? { ...item, status: "completed" as const, completedAtUtc: new Date().toISOString() }
            : item
        );

        const nextProperty = {
          ...property,
          tasks: nextTasksState,
          taskSummary: updateTaskSummary(nextTasksState)
        };

        return nextProperty;
      })
    );

    completeTaskMutation.mutate(task);
  };

  const markConditionComplete = (condition: Condition) => {
    if (condition.status === "satisfied" || condition.status === "waived") {
      return;
    }

    setProperties((prev) =>
      prev.map((property) => {
        if (property.id !== selectedProperty.id) {
          return property;
        }

        const nextConditions = property.conditions.map((item) =>
          item.id === condition.id
            ? { ...item, status: "satisfied" as const, completedAtUtc: new Date().toISOString() }
            : item
        );

        const nextProperty = {
          ...property,
          conditions: nextConditions
        };

        return nextProperty;
      })
    );

    completeConditionMutation.mutate(condition);
  };

  const taskGroups = groupedTasks(allTasks);

  return (
    <div className="space-y-6">
      <PurchaseHeroCard
        property={selectedProperty}
        progressValue={progressValue}
        canSelect={properties.length > 1}
        properties={properties}
        onSelectProperty={setSelectedPropertyId}
      />

      <StageTimeline currentStatus={selectedProperty.status} />

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="order-1 space-y-6 lg:col-span-2">
          <div className="space-y-4">
            <Tabs value={taskFilter} onValueChange={(value) => setTaskFilter(value as TaskFilter)}>
              <TabsList>
                <TabsTrigger value="all">All</TabsTrigger>
                <TabsTrigger value="next">Next</TabsTrigger>
                <TabsTrigger value="completed">Completed</TabsTrigger>
              </TabsList>
            </Tabs>
            <NextActionsCard
              tasks={filteredTasks.slice(0, 5)}
              disabled={completeTaskMutation.isPending}
              onToggleTask={markTaskComplete}
            />
          </div>
        </div>

        <div className="order-2 space-y-6 lg:col-span-1">
          <SettlementCountdownCard
            settlementDate={selectedProperty.settlementDate}
            daysUntilSettlement={selectedProperty.daysUntilSettlement}
          />

          <ConditionsCard
            conditions={selectedProperty.conditions}
            disabled={completeConditionMutation.isPending}
            onCompleteCondition={markConditionComplete}
          />

          <ProgressCard
            completed={selectedProperty.taskSummary.completed}
            total={selectedProperty.taskSummary.total}
            pending={selectedProperty.taskSummary.pending}
            openConditions={selectedProperty.conditions.filter((condition) => condition.status !== "satisfied" && condition.status !== "waived").length}
          />
        </div>

        <div className="order-3 lg:col-span-2">
          <TaskList groupedTasks={taskGroups} disabled={completeTaskMutation.isPending} onToggleTask={markTaskComplete} />
        </div>
      </div>
    </div>
  );
}
