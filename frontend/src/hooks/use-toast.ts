"use client";

import * as React from "react";

type Toast = {
  id: string;
  title?: string;
  description?: string;
  variant?: "default" | "success" | "warning" | "danger";
  duration?: number;
};

type ToastState = {
  toasts: Toast[];
};

const listeners: Array<(state: ToastState) => void> = [];
let memoryState: ToastState = { toasts: [] };

function emit(nextState: ToastState) {
  memoryState = nextState;
  listeners.forEach((listener) => listener(nextState));
}

function removeToast(id: string) {
  emit({
    toasts: memoryState.toasts.filter((toast) => toast.id !== id)
  });
}

function toast({
  title,
  description,
  variant = "default",
  duration = 3500
}: Omit<Toast, "id">) {
  const id = crypto.randomUUID();

  emit({
    toasts: [...memoryState.toasts, { id, title, description, variant, duration }]
  });

  return {
    id,
    dismiss: () => removeToast(id)
  };
}

function useToast() {
  const [state, setState] = React.useState<ToastState>(memoryState);

  React.useEffect(() => {
    listeners.push(setState);

    return () => {
      const index = listeners.indexOf(setState);
      if (index > -1) {
        listeners.splice(index, 1);
      }
    };
  }, [setState]);

  return {
    toasts: state.toasts,
    toast,
    dismiss: removeToast
  };
}

export { toast, useToast };