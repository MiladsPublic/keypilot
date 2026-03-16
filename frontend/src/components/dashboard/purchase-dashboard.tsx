"use client";

import Link from "next/link";
import { useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@clerk/nextjs";
import { useMutation } from "@tanstack/react-query";

import { completeCondition } from "@/features/properties/api/complete-condition";
import { completeTask } from "@/features/properties/api/complete-task";
import { type Condition, type Property, type PropertyTask } from "@/features/properties/types/property";

type TaskFilter = "all" | "next" | "completed";

const timelineStages = ["accepted_offer", "conditional", "unconditional", "pre_settlement", "settled"] as const;

function formatStage(value: Property["status"]) {
  return value
    .split("_")
    .map((part) => part[0].toUpperCase() + part.slice(1))
    .join(" ");
}

function formatDate(value: string | null) {
  if (!value) {
    return "No date";
  }

  const normalizedValue = value.includes("T") ? value : `${value}T00:00:00`;

  return new Intl.DateTimeFormat("en-NZ", {
    day: "numeric",
    month: "short",
    year: "numeric"
  }).format(new Date(normalizedValue));
}

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

function deriveStatus(property: Property): Property["status"] {
  if (property.status === "settled") {
    return "settled";
  }

  if (property.conditions.some((condition) => condition.status === "pending")) {
    return "conditional";
  }

  if (property.daysUntilSettlement < 7) {
    return "pre_settlement";
  }

  return "unconditional";
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
    mutationFn: async (taskId: string) => completeTask(taskId, await getToken()),
    onSuccess: () => router.refresh()
  });

  const completeConditionMutation = useMutation({
    mutationFn: async (conditionId: string) => completeCondition(conditionId, await getToken()),
    onSuccess: () => router.refresh()
  });

  if (!selectedProperty) {
    return null;
  }

  const currentStageIndex = timelineStages.indexOf(selectedProperty.status);

  const allTasks = sortTasks(selectedProperty.tasks);
  const pendingTasks = allTasks.filter((task) => task.status === "pending");
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

  const nextTasks = (dueSoonTasks.length > 0 ? dueSoonTasks : pendingTasks).slice(0, 8);
  const completedTasks = allTasks.filter((task) => task.status === "completed");

  const filteredTasks =
    taskFilter === "all" ? allTasks : taskFilter === "completed" ? completedTasks : nextTasks;

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

        return {
          ...nextProperty,
          status: deriveStatus(nextProperty)
        };
      })
    );

    completeTaskMutation.mutate(task.id);
  };

  const markConditionComplete = (condition: Condition) => {
    if (condition.status === "completed") {
      return;
    }

    setProperties((prev) =>
      prev.map((property) => {
        if (property.id !== selectedProperty.id) {
          return property;
        }

        const nextConditions = property.conditions.map((item) =>
          item.id === condition.id
            ? { ...item, status: "completed" as const, completedAtUtc: new Date().toISOString() }
            : item
        );

        const nextProperty = {
          ...property,
          conditions: nextConditions
        };

        return {
          ...nextProperty,
          status: deriveStatus(nextProperty)
        };
      })
    );

    completeConditionMutation.mutate(condition.id);
  };

  const countdownClass = selectedProperty.daysUntilSettlement < 7 ? "text-red-700" : "text-ink";

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <p className="text-sm font-semibold uppercase tracking-[0.22em] text-accent">Dashboard</p>
          <h1 className="font-[family-name:var(--font-display)] text-4xl">Purchase control panel</h1>
        </div>
        <div className="flex items-center gap-3">
          {properties.length > 1 ? (
            <select
              value={selectedProperty.id}
              onChange={(event) => setSelectedPropertyId(event.target.value)}
              className="rounded-full border border-line bg-white px-4 py-3 text-sm"
            >
              {properties.map((property) => (
                <option key={property.id} value={property.id}>
                  {property.address}
                </option>
              ))}
            </select>
          ) : null}
          <Link
            href="/properties/new"
            className="inline-flex items-center justify-center rounded-full border border-line bg-white/80 px-4 py-3 text-sm font-semibold text-ink transition hover:bg-white"
          >
            Create purchase
          </Link>
        </div>
      </div>

      <section className="rounded-2xl border border-line bg-white p-6">
        <p className="text-sm font-semibold uppercase tracking-[0.16em] text-ink/60">Current purchase</p>
        <h2 className="mt-2 text-2xl font-semibold text-ink">{selectedProperty.address}</h2>
        <p className="mt-2 text-sm text-ink/75">Stage: {formatStage(selectedProperty.status)}</p>
        <p className={`mt-1 text-sm font-semibold ${countdownClass}`}>
          Settlement: {selectedProperty.daysUntilSettlement} days away
        </p>
      </section>

      <section className="rounded-2xl border border-line bg-white p-6">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <h3 className="text-lg font-semibold text-ink">Next Actions</h3>
          <div className="flex flex-wrap gap-2 text-xs">
            <button
              type="button"
              onClick={() => setTaskFilter("all")}
              className={`rounded-full border px-3 py-2 ${taskFilter === "all" ? "border-ink bg-ink text-white" : "border-line bg-white text-ink/70"}`}
            >
              All Tasks
            </button>
            <button
              type="button"
              onClick={() => setTaskFilter("next")}
              className={`rounded-full border px-3 py-2 ${taskFilter === "next" ? "border-ink bg-ink text-white" : "border-line bg-white text-ink/70"}`}
            >
              Next Tasks
            </button>
            <button
              type="button"
              onClick={() => setTaskFilter("completed")}
              className={`rounded-full border px-3 py-2 ${taskFilter === "completed" ? "border-ink bg-ink text-white" : "border-line bg-white text-ink/70"}`}
            >
              Completed
            </button>
          </div>
        </div>

        <div className="mt-4 space-y-3">
          {filteredTasks.length === 0 ? <p className="text-sm text-ink/65">No tasks in this view.</p> : null}
          {filteredTasks.map((task) => (
            <label key={task.id} className="flex items-center justify-between gap-3 rounded-xl border border-line bg-canvas px-3 py-3">
              <div className="flex items-center gap-3">
                <input
                  type="checkbox"
                  checked={task.status === "completed"}
                  disabled={task.status === "completed" || completeTaskMutation.isPending}
                  onChange={() => markTaskComplete(task)}
                />
                <div>
                  <p className="text-sm font-semibold text-ink">{task.title}</p>
                  <p className="text-xs text-ink/65">{task.dueDate ? `Due ${formatDate(task.dueDate)}` : "No due date"}</p>
                </div>
              </div>
              <span className="text-xs uppercase tracking-[0.08em] text-ink/60">{task.status}</span>
            </label>
          ))}
        </div>
      </section>

      <section className="rounded-2xl border border-line bg-white p-6">
        <h3 className="text-lg font-semibold text-ink">Conditions</h3>
        <div className="mt-4 space-y-3">
          {selectedProperty.conditions.map((condition) => (
            <div key={condition.id} className="flex flex-wrap items-center justify-between gap-3 rounded-xl border border-line bg-canvas px-3 py-3">
              <Link href={`/properties/${selectedProperty.id}#condition-${condition.id}`} className="text-sm font-semibold text-ink hover:underline">
                {condition.type}
              </Link>
              <p className="text-xs text-ink/65">Due: {formatDate(condition.dueDate)}</p>
              <p className="text-xs uppercase tracking-[0.08em] text-ink/65">{condition.status}</p>
              <button
                type="button"
                onClick={() => markConditionComplete(condition)}
                disabled={condition.status === "completed" || completeConditionMutation.isPending}
                className="rounded-full border border-line bg-white px-3 py-2 text-xs font-semibold text-ink/80 disabled:opacity-50"
              >
                Mark Complete
              </button>
            </div>
          ))}
        </div>
      </section>

      <section className="rounded-2xl border border-line bg-white p-6">
        <h3 className="text-lg font-semibold text-ink">Progress Timeline</h3>
        <div className="mt-4 grid gap-2 md:grid-cols-5">
          {timelineStages.map((stage, index) => {
            const isComplete = index < currentStageIndex;
            const isCurrent = index === currentStageIndex;

            return (
              <div key={stage} className="rounded-xl border border-line bg-canvas px-3 py-3 text-sm">
                <p className="font-semibold text-ink">{formatStage(stage)}</p>
                <p className="mt-1 text-xs text-ink/65">{isComplete ? "Completed" : isCurrent ? "Current" : "Upcoming"}</p>
              </div>
            );
          })}
        </div>
      </section>
    </div>
  );
}
