import { forwardRef } from "react";

import { cn } from "@/lib/utils";

export const Input = forwardRef<HTMLInputElement, React.InputHTMLAttributes<HTMLInputElement>>(
  function Input({ className, ...props }, ref) {
    return (
      <input
        ref={ref}
        className={cn(
          "w-full rounded-2xl border border-line bg-white px-4 py-3 text-sm text-ink outline-none transition",
          "placeholder:text-ink/35 focus:border-accent focus:ring-2 focus:ring-accent/20",
          className
        )}
        {...props}
      />
    );
  }
);
