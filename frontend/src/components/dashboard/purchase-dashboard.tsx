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

function formatDueLabel(value: string | null) {
  if (!value) {
    return "No due date";
  }

  const dueDate = new Date(`${value}T00:00:00`).getTime();
  const today = new Date();
  const now = new Date(today.getFullYear(), today.getMonth(), today.getDate()).getTime();
  const diffDays = Math.floor((dueDate - now) / (1000 * 60 * 60 * 24));

  if (diffDays === 0) {
    return "Due today";
  }

  if (diffDays === 1) {
    return "Due tomorrow";
  }

  if (diffDays > 1 && diffDays <= 7) {
    return `Due in ${diffDays} days`;
  }

  if (diffDays < 0) {
    return `Overdue by ${Math.abs(diffDays)} days`;
  }

  return `Due ${formatDate(value)}`;
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

  const addressParts = selectedProperty.address.split(",").map((part) => part.trim()).filter(Boolean);
  const headlineAddress = addressParts[0] ?? selectedProperty.address;
  const subAddress = addressParts.slice(1).join(", ");

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
    <div className="space-y-5">
      <section className="rounded-2xl border border-line bg-white px-6 py-7 shadow-[0_1px_2px_rgba(0,0,0,0.03)]">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <p className="text-xs font-medium uppercase tracking-[0.14em] text-ink/55">My Purchase</p>
            <h1 className="mt-2 text-[28px] font-semibold leading-tight text-ink">{headlineAddress}</h1>
            {subAddress ? <p className="mt-1 text-sm text-ink/70">{subAddress}</p> : null}
            <p className="mt-3 text-sm text-ink/80">
              {formatStage(selectedProperty.status)}
              <span className={`ml-2 font-semibold ${countdownClass}`}>
                • Settlement in {selectedProperty.daysUntilSettlement} days
              </span>
            </p>
          </div>

          <div className="flex items-center gap-3">
            {properties.length > 1 ? (
              <select
                value={selectedProperty.id}
                onChange={(event) => setSelectedPropertyId(event.target.value)}
                className="rounded-xl border border-line bg-white px-3 py-2 text-sm"
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
              className="inline-flex items-center justify-center rounded-xl border border-line bg-white px-3 py-2 text-sm font-medium text-ink transition hover:bg-canvas"
            >
              Create Purchase
            </Link>
          </div>
        </div>
      </section>

      <section className="rounded-2xl border border-line bg-white p-6 shadow-[0_1px_2px_rgba(0,0,0,0.03)]">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <h3 className="text-base font-semibold text-ink">Next Actions</h3>
          <div className="flex flex-wrap gap-2 text-xs">
            <button
              type="button"
              onClick={() => setTaskFilter("all")}
              className={`rounded-lg border px-3 py-1.5 ${taskFilter === "all" ? "border-ink bg-ink text-white" : "border-line bg-white text-ink/70"}`}
            >
              All Tasks
            </button>
            <button
              type="button"
              onClick={() => setTaskFilter("next")}
              className={`rounded-lg border px-3 py-1.5 ${taskFilter === "next" ? "border-ink bg-ink text-white" : "border-line bg-white text-ink/70"}`}
            >
              Next Tasks
            </button>
            <button
              type="button"
              onClick={() => setTaskFilter("completed")}
              className={`rounded-lg border px-3 py-1.5 ${taskFilter === "completed" ? "border-ink bg-ink text-white" : "border-line bg-white text-ink/70"}`}
            >
              Completed
            </button>
          </div>
        </div>

        <div className="mt-4 space-y-2.5">
          {filteredTasks.length === 0 ? <p className="text-sm text-ink/65">No tasks in this view.</p> : null}
          {filteredTasks.map((task) => (
            <label key={task.id} className="flex items-start gap-3 rounded-xl px-2 py-2 hover:bg-canvas/80">
              <input
                type="checkbox"
                className="mt-1 h-4 w-4 rounded border-line"
                checked={task.status === "completed"}
                disabled={task.status === "completed" || completeTaskMutation.isPending}
                onChange={() => markTaskComplete(task)}
              />
              <div>
                <p className="text-[15px] font-medium text-ink">{task.title}</p>
                <p className="text-[13px] text-ink/65">{formatDueLabel(task.dueDate)}</p>
              </div>
            </label>
          ))}
        </div>
      </section>

      <section className="rounded-2xl border border-line bg-white p-6 shadow-[0_1px_2px_rgba(0,0,0,0.03)]">
        <h3 className="text-base font-semibold text-ink">Conditions</h3>
        <div className="mt-4 space-y-2.5">
          {selectedProperty.conditions.map((condition) => (
            <div key={condition.id} className="flex flex-wrap items-center justify-between gap-3 rounded-xl border border-line px-3 py-3">
              <div>
                <Link href={`/properties/${selectedProperty.id}#condition-${condition.id}`} className="text-sm font-medium text-ink hover:underline">
                  {condition.type}
                </Link>
                <p className="text-xs text-ink/65">Due {formatDate(condition.dueDate)}</p>
              </div>
              <div className="flex items-center gap-2">
                <p className="text-xs uppercase tracking-[0.08em] text-ink/65">{condition.status}</p>
                <button
                  type="button"
                  onClick={() => markConditionComplete(condition)}
                  disabled={condition.status === "completed" || completeConditionMutation.isPending}
                  className="rounded-lg border border-line bg-white px-3 py-1.5 text-xs font-medium text-ink/80 disabled:opacity-50"
                >
                  Mark Complete
                </button>
              </div>
            </div>
          ))}
        </div>
      </section>

      <section className="rounded-2xl border border-line bg-white p-6 shadow-[0_1px_2px_rgba(0,0,0,0.03)]">
        <h3 className="text-base font-semibold text-ink">Progress</h3>
        <div className="mt-5 grid gap-3 md:grid-cols-5">
          {timelineStages.map((stage, index) => {
            const isComplete = index < currentStageIndex;
            const isCurrent = index === currentStageIndex;

            return (
              <div key={stage} className="rounded-xl border border-line bg-white px-3 py-3">
                <div className="flex items-center gap-2">
                  <span
                    className={`inline-flex h-3 w-3 rounded-full ${isComplete ? "bg-accent" : isCurrent ? "border-2 border-accent" : "border border-line bg-white"}`}
                  />
                  <p className="text-sm font-medium text-ink">{formatStage(stage)}</p>
                </div>
                <p className="mt-1 text-xs text-ink/60">{isComplete ? "Completed" : isCurrent ? "Current" : "Upcoming"}</p>
              </div>
            );
          })}
        </div>
      </section>
    </div>
  );
}
