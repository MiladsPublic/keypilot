import { cn } from "@/lib/utils";

export function Badge(props: React.HTMLAttributes<HTMLSpanElement>) {
  return (
    <span
      {...props}
      className={cn(
        "inline-flex items-center rounded-full bg-accent/12 px-3 py-1 text-xs font-semibold uppercase tracking-[0.18em] text-accent",
        props.className
      )}
    />
  );
}
