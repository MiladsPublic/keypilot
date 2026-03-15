import { forwardRef } from "react";

import { cn } from "@/lib/utils";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "primary" | "secondary";
};

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(function Button(
  { className, variant = "primary", type = "button", ...props },
  ref
) {
  return (
    <button
      ref={ref}
      type={type}
      className={cn(
        "inline-flex items-center justify-center rounded-full px-4 py-3 text-sm font-semibold transition-transform duration-150",
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-accent focus-visible:ring-offset-2 focus-visible:ring-offset-transparent",
        variant === "primary" &&
          "bg-ink text-white shadow-panel hover:-translate-y-0.5 hover:bg-[#15231d]",
        variant === "secondary" && "border border-line bg-white/80 text-ink hover:bg-white",
        props.disabled && "cursor-not-allowed opacity-60 hover:translate-y-0",
        className
      )}
      {...props}
    />
  );
});
