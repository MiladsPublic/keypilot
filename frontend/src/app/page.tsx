import Link from "next/link";
import { ArrowRight, BellRing, CheckCircle2, FileText, ListChecks } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

const highlights = [
  {
    title: "Track deadlines",
    description: "Keep conditional dates and settlement timing in one place.",
    icon: BellRing
  },
  {
    title: "Stay organised",
    description: "Keep tasks, updates and key milestones in one clear timeline.",
    icon: ListChecks
  },
  {
    title: "Reduce inbox chaos",
    description: "Bring documents and deadlines into one calm purchase workspace.",
    icon: FileText
  }
];

const journey = ["Offer accepted", "Conditional", "Unconditional", "Pre-settlement", "Settled"];

export default function HomePage() {
  return (
    <div className="space-y-6">
      <Card className="rounded-2xl">
        <CardContent className="grid gap-6 p-5 md:grid-cols-[1.4fr_1fr] md:p-6">
          <div className="space-y-4">
            <h1 className="max-w-2xl text-5xl font-semibold leading-tight md:text-6xl">
              Guide each home purchase from accepted offer to settlement.
            </h1>
            <p className="max-w-xl text-base leading-7 text-ink/70">
              KeyPilot keeps the purchase timeline in one calm workspace - conditions, deadlines, tasks and documents.
            </p>
            <div className="flex flex-wrap items-center gap-2">
              <Button asChild className="rounded-full px-5">
                <Link href="/properties/new">
                  Start a purchase
                  <ArrowRight className="h-4 w-4" />
                </Link>
              </Button>
              <Button asChild variant="outline" className="rounded-lg">
                <Link href="/sign-in">Sign in</Link>
              </Button>
            </div>
          </div>

          <Card className="rounded-2xl border border-line bg-white shadow-sm">
            <CardHeader>
              <CardTitle className="text-lg">Purchase at a glance</CardTitle>
              <CardDescription>What stage are you in and what needs attention next.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="rounded-xl border border-line bg-[var(--muted)] p-3 text-sm">Current stage: Conditional</div>
              <div className="rounded-xl border border-line bg-[var(--muted)] p-3 text-sm">Settlement in 12 days</div>
              <div className="rounded-xl border border-line bg-[var(--muted)] p-3 text-sm">3 conditions still open</div>
              <div className="rounded-xl border border-line bg-[var(--muted)] p-3 text-sm">4 / 12 tasks completed</div>
            </CardContent>
          </Card>
        </CardContent>
      </Card>

      <section className="grid gap-6 md:grid-cols-3">
        {highlights.map((highlight) => {
          const Icon = highlight.icon;

          return (
            <Card key={highlight.title} className="rounded-2xl">
              <CardHeader>
                <div className="mb-2 inline-flex w-fit rounded-full bg-[var(--muted)] p-2">
                  <Icon className="h-4 w-4 text-accent" />
                </div>
                <CardTitle className="text-lg">{highlight.title}</CardTitle>
                <CardDescription>{highlight.description}</CardDescription>
              </CardHeader>
            </Card>
          );
        })}
      </section>

      <Card className="rounded-2xl">
        <CardHeader>
          <CardTitle className="text-lg">Purchase journey</CardTitle>
          <CardDescription>See the full path from accepted offer to settlement.</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-wrap items-center gap-3">
          {journey.map((step) => (
            <div key={step} className="inline-flex items-center gap-2 rounded-full border border-line bg-white px-3 py-2 text-sm">
              <CheckCircle2 className="h-4 w-4 text-ink/55" />
              {step}
            </div>
          ))}
        </CardContent>
      </Card>

      <Card className="rounded-2xl">
        <CardContent className="flex flex-wrap items-center justify-between gap-3 p-6">
          <div>
            <p className="text-lg font-semibold">Start your purchase workspace today</p>
            <p className="text-sm text-ink/70">Keep the full purchase timeline visible from day one.</p>
          </div>
          <Button asChild className="rounded-full px-5">
            <Link href="/properties/new">Start a purchase</Link>
          </Button>
        </CardContent>
      </Card>
    </div>
  );
}
