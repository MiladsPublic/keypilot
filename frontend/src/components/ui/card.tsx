import { cn } from "@/lib/utils";

export function Card(props: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div
      {...props}
      className={cn(
        "rounded-panel border border-line bg-white/85 shadow-panel backdrop-blur-sm",
        props.className
      )}
    />
  );
}

export function CardHeader(props: React.HTMLAttributes<HTMLDivElement>) {
  return <div {...props} className={cn("space-y-2 p-6", props.className)} />;
}

export function CardTitle(props: React.HTMLAttributes<HTMLHeadingElement>) {
  return <h2 {...props} className={cn("text-xl font-semibold tracking-tight", props.className)} />;
}

export function CardDescription(props: React.HTMLAttributes<HTMLParagraphElement>) {
  return <p {...props} className={cn("text-sm text-ink/70", props.className)} />;
}

export function CardContent(props: React.HTMLAttributes<HTMLDivElement>) {
  return <div {...props} className={cn("p-6 pt-0", props.className)} />;
}
