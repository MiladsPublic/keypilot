import { type Property } from "@/features/properties/types/property";

export const timelineStages = [
  "accepted_offer",
  "conditional",
  "unconditional",
  "pre_settlement",
  "settled",
  "cancelled"
] as const;

export function formatStage(value: string) {
  const normalized = value === "settlement" ? "settled" : value;

  return normalized
    .split("_")
    .map((part) => part[0].toUpperCase() + part.slice(1))
    .join(" ");
}

export function formatDate(value: string | null) {
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

export function formatDueLabel(value: string | null) {
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

export function dueTone(value: string | null): "normal" | "warning" | "danger" {
  if (!value) {
    return "normal";
  }

  const dueDate = new Date(`${value}T00:00:00`).getTime();
  const today = new Date();
  const now = new Date(today.getFullYear(), today.getMonth(), today.getDate()).getTime();
  const diffDays = Math.floor((dueDate - now) / (1000 * 60 * 60 * 24));

  if (diffDays < 0 || diffDays < 7) {
    return "danger";
  }

  if (diffDays <= 14) {
    return "warning";
  }

  return "normal";
}

export function countdownTone(daysUntilSettlement: number): "normal" | "warning" | "danger" {
  if (daysUntilSettlement < 7) {
    return "danger";
  }

  if (daysUntilSettlement <= 14) {
    return "warning";
  }

  return "normal";
}

export function badgeVariantForStatus(
  status: string
): "default" | "secondary" | "warning" | "success" | "danger" {
  if (status === "settled" || status === "completed" || status === "satisfied" || status === "waived") {
    return "success";
  }

  if (status === "expired" || status === "failed" || status === "cancelled") {
    return "danger";
  }

  if (status === "conditional" || status === "pending" || status === "pre_settlement") {
    return "warning";
  }

  return "secondary";
}
