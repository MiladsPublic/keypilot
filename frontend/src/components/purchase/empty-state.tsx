import Link from "next/link";

import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";

type EmptyStateProps = {
  message: string;
  actionLabel?: string;
  actionHref?: string;
};

export function EmptyState({ message, actionLabel, actionHref }: EmptyStateProps) {
  return (
    <Card className="rounded-2xl">
      <CardContent className="space-y-4 p-8">
        <p className="text-sm text-ink/70">{message}</p>
        {actionLabel && actionHref ? (
          <Button asChild className="rounded-full">
            <Link href={actionHref}>{actionLabel}</Link>
          </Button>
        ) : null}
      </CardContent>
    </Card>
  );
}
