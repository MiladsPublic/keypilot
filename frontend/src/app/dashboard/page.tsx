import Link from "next/link";
import { auth } from "@clerk/nextjs/server";

import { getProperties } from "@/features/properties/api/get-properties";

export default async function DashboardPage() {
  const { getToken } = await auth();
  const token = await getToken();
  const properties = await getProperties(token);

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <p className="text-sm font-semibold uppercase tracking-[0.22em] text-accent">Dashboard</p>
          <h1 className="font-[family-name:var(--font-display)] text-4xl">Your properties</h1>
        </div>
        <Link
          href="/properties/new"
          className="inline-flex items-center justify-center rounded-full border border-line bg-white/80 px-4 py-3 text-sm font-semibold text-ink transition hover:bg-white"
        >
          Create property
        </Link>
      </div>

      {properties.length === 0 ? (
        <div className="rounded-2xl border border-line bg-white p-6 text-sm text-ink/70">
          No properties yet. Create your first workspace to start tracking the purchase journey.
        </div>
      ) : (
        <div className="grid gap-4 md:grid-cols-2">
          {properties.map((property) => (
            <Link
              key={property.id}
              href={`/properties/${property.id}`}
              className="rounded-2xl border border-line bg-white p-5 transition hover:-translate-y-0.5 hover:shadow-panel"
            >
              <p className="text-xs uppercase tracking-[0.16em] text-ink/55">{property.status}</p>
              <h2 className="mt-2 text-lg font-semibold text-ink">{property.address}</h2>
              <p className="mt-2 text-sm text-ink/70">Settlement: {property.settlementDate}</p>
              <p className="mt-1 text-sm text-ink/70">
                Tasks: {property.taskSummary.completed}/{property.taskSummary.total} complete
              </p>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
