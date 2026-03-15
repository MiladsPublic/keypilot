import Link from "next/link";
import { ArrowRight, BellRing, FileText, ListTodo } from "lucide-react";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

const highlights = [
  {
    title: "Track deadlines",
    description: "Keep conditional dates and settlement timing in one place.",
    icon: BellRing
  },
  {
    title: "Stay organised",
    description: "Tasks, contacts, and documents can grow around the property workspace.",
    icon: ListTodo
  },
  {
    title: "Reduce inbox chaos",
    description: "Bring the buying journey out of scattered email threads and PDFs.",
    icon: FileText
  }
];

export default function HomePage() {
  return (
    <div className="space-y-8">
      <section className="grid gap-6 rounded-[2rem] border border-line bg-white/75 p-6 shadow-panel backdrop-blur-sm md:grid-cols-[1.4fr_1fr] md:p-10">
        <div className="space-y-5">
          <p className="text-sm font-semibold uppercase tracking-[0.22em] text-accent">Home buying workspace</p>
          <h1 className="max-w-xl font-[family-name:var(--font-display)] text-4xl leading-tight md:text-6xl">
            Guide each property from accepted offer to settlement.
          </h1>
          <p className="max-w-xl text-base leading-7 text-ink/72">
            KeyPilot is the operational layer for the purchase journey: timeline, tasks, documents, and the people around the deal.
          </p>
          <Link
            href="/properties/new"
            className="mt-2 inline-flex w-fit items-center justify-center rounded-full bg-ink px-4 py-3 text-sm font-semibold text-white shadow-panel transition hover:-translate-y-0.5 hover:bg-[#15231d]"
          >
            Start a property
            <ArrowRight className="ml-2 h-4 w-4" />
          </Link>
        </div>

        <div className="rounded-[2rem] bg-ink p-6 text-white">
          <p className="text-sm uppercase tracking-[0.22em] text-white/70">MVP focus</p>
          <div className="mt-6 space-y-4">
            <div>
              <p className="text-3xl font-semibold">Property workspace</p>
              <p className="mt-2 text-sm text-white/70">Start with the core record and build the rest around it.</p>
            </div>
            <div className="grid grid-cols-2 gap-3 text-sm">
              <div className="rounded-3xl bg-white/10 p-4">Conditions</div>
              <div className="rounded-3xl bg-white/10 p-4">Tasks</div>
              <div className="rounded-3xl bg-white/10 p-4">Documents</div>
              <div className="rounded-3xl bg-white/10 p-4">Contacts</div>
            </div>
          </div>
        </div>
      </section>

      <section className="grid gap-4 md:grid-cols-3">
        {highlights.map((highlight) => {
          const Icon = highlight.icon;

          return (
            <Card key={highlight.title}>
              <CardHeader>
                <div className="mb-2 inline-flex w-fit rounded-2xl bg-accent/10 p-3 text-accent">
                  <Icon className="h-5 w-5" />
                </div>
                <CardTitle>{highlight.title}</CardTitle>
                <CardDescription>{highlight.description}</CardDescription>
              </CardHeader>
              <CardContent className="pt-0 text-sm text-ink/60">
                The scaffold already includes the property creation flow and the backend contract it depends on.
              </CardContent>
            </Card>
          );
        })}
      </section>
    </div>
  );
}
