"use client";

import { useMemo } from "react";
import { useRouter } from "next/navigation";
import { useMutation } from "@tanstack/react-query";
import { useAuth } from "@clerk/nextjs";
import { CalendarClock, CheckCircle2, ListTodo, MapPinHouse, Wallet } from "lucide-react";

import { completeCondition } from "@/features/properties/api/complete-condition";
import { completeTask } from "@/features/properties/api/complete-task";
import { settleProperty } from "@/features/properties/api/settle-property";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { type Property } from "@/features/properties/types/property";

function formatDate(value: string | null) {
  if (!value) {
    return "Not set";
  }

  const normalizedValue = value.includes("T") ? value : `${value}T00:00:00`;

  return new Intl.DateTimeFormat("en-NZ", {
    day: "numeric",
    month: "short",
    year: "numeric"
  }).format(new Date(normalizedValue));
}

function formatCurrency(value: number | null) {
  if (value === null) {
    return "Not set";
  }

  return new Intl.NumberFormat("en-NZ", {
    style: "currency",
    currency: "NZD",
    maximumFractionDigits: 0
  }).format(value);
}

export function PropertySummaryCard({ property }: { property: Property }) {
  const router = useRouter();
  const { getToken } = useAuth();

  const taskMutation = useMutation({
    mutationFn: async (id: string) => completeTask(id, await getToken()),
    onSuccess: () => router.refresh()
  });

  const conditionMutation = useMutation({
    mutationFn: async (id: string) => completeCondition(id, await getToken()),
    onSuccess: () => router.refresh()
  });

  const settleMutation = useMutation({
    mutationFn: async (id: string) => settleProperty(id, await getToken()),
    onSuccess: () => router.refresh()
  });

  const tasksByStage = useMemo(() => {
    const stages = ["accepted_offer", "conditional", "unconditional", "pre_settlement", "settlement"] as const;

    return stages.map((stage) => ({
      stage,
      tasks: property.tasks.filter((task) => task.stage === stage)
    }));
  }, [property.tasks]);

  return (
    <Card className="space-y-4 p-1">
      <CardHeader>
        <div className="flex items-start justify-between gap-4">
          <div className="space-y-2">
            <Badge>{property.status}</Badge>
            <CardTitle className="text-2xl">{property.address}</CardTitle>
          </div>
          <div className="rounded-full border border-line bg-canvas px-3 py-2 text-xs font-medium text-ink/70">
            Created {formatDate(property.createdAtUtc)}
          </div>
        </div>
      </CardHeader>
      <CardContent className="grid gap-4 md:grid-cols-4">
        <div className="rounded-3xl bg-canvas p-4">
          <MapPinHouse className="mb-3 h-5 w-5 text-accent" />
          <p className="text-xs uppercase tracking-[0.18em] text-ink/55">Address</p>
          <p className="mt-2 text-sm font-semibold">{property.address}</p>
        </div>
        <div className="rounded-3xl bg-canvas p-4">
          <CalendarClock className="mb-3 h-5 w-5 text-accent" />
          <p className="text-xs uppercase tracking-[0.18em] text-ink/55">Timeline</p>
          <p className="mt-2 text-sm font-semibold">Offer accepted: {formatDate(property.acceptedOfferDate)}</p>
          <p className="mt-1 text-sm font-semibold">Settlement: {formatDate(property.settlementDate)}</p>
          <p className="mt-1 text-sm text-ink/70">Days until settlement: {property.daysUntilSettlement}</p>
        </div>
        <div className="rounded-3xl bg-canvas p-4">
          <Wallet className="mb-3 h-5 w-5 text-accent" />
          <p className="text-xs uppercase tracking-[0.18em] text-ink/55">Purchase price</p>
          <p className="mt-2 text-sm font-semibold">{formatCurrency(property.purchasePrice)}</p>
        </div>
        <div className="rounded-3xl bg-canvas p-4">
          <CheckCircle2 className="mb-3 h-5 w-5 text-accent" />
          <p className="text-xs uppercase tracking-[0.18em] text-ink/55">Task progress</p>
          <p className="mt-2 text-sm font-semibold">
            {property.taskSummary.completed}/{property.taskSummary.total} completed
          </p>
          <p className="mt-1 text-sm text-ink/70">{property.taskSummary.pending} remaining</p>
        </div>
      </CardContent>

      <CardContent className="space-y-4">
        <div className="flex flex-wrap items-center justify-between gap-3 rounded-2xl border border-line bg-canvas p-4">
          <div>
            <p className="text-xs uppercase tracking-[0.18em] text-ink/55">Current stage</p>
            <p className="mt-1 text-sm font-semibold">{property.status}</p>
          </div>
          <Button
            type="button"
            variant="secondary"
            disabled={property.status === "settled" || settleMutation.isPending}
            onClick={() => settleMutation.mutate(property.id)}
          >
            Mark settlement complete
          </Button>
        </div>

        <section className="space-y-3">
          <div className="flex items-center gap-2">
            <CheckCircle2 className="h-4 w-4 text-accent" />
            <h3 className="text-base font-semibold">Conditions</h3>
          </div>
          <div className="grid gap-3 md:grid-cols-2">
            {property.conditions.map((condition) => (
              <div key={condition.id} className="rounded-2xl border border-line bg-white p-4">
                <p className="text-sm font-semibold">{condition.type}</p>
                <p className="mt-1 text-sm text-ink/70">Due: {formatDate(condition.dueDate)}</p>
                <p className="mt-1 text-sm text-ink/70">Status: {condition.status}</p>
                <Button
                  type="button"
                  variant="secondary"
                  className="mt-3"
                  disabled={condition.status === "completed" || conditionMutation.isPending}
                  onClick={() => conditionMutation.mutate(condition.id)}
                >
                  Mark complete
                </Button>
              </div>
            ))}
          </div>
        </section>

        <section className="space-y-3">
          <div className="flex items-center gap-2">
            <ListTodo className="h-4 w-4 text-accent" />
            <h3 className="text-base font-semibold">Tasks by stage</h3>
          </div>
          <div className="space-y-3">
            {tasksByStage.map((entry) => (
              <div key={entry.stage} className="rounded-2xl border border-line bg-white p-4">
                <p className="text-sm font-semibold uppercase tracking-[0.08em] text-ink/70">{entry.stage}</p>
                <div className="mt-3 space-y-3">
                  {entry.tasks.length === 0 ? <p className="text-sm text-ink/60">No tasks in this stage.</p> : null}
                  {entry.tasks.map((task) => (
                    <div key={task.id} className="flex flex-wrap items-center justify-between gap-2 rounded-xl bg-canvas px-3 py-2">
                      <div>
                        <p className="text-sm font-semibold">{task.title}</p>
                        <p className="text-xs text-ink/65">
                          {task.dueDate ? `Due ${formatDate(task.dueDate)} • ` : ""}
                          {task.status}
                        </p>
                      </div>
                      <Button
                        type="button"
                        variant="secondary"
                        disabled={task.status === "completed" || taskMutation.isPending}
                        onClick={() => taskMutation.mutate(task.id)}
                      >
                        Complete
                      </Button>
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>
        </section>
      </CardContent>
    </Card>
  );
}
