import Link from "next/link";
import { auth } from "@clerk/nextjs/server";
import { redirect } from "next/navigation";

import { PurchaseDashboard } from "@/components/dashboard/purchase-dashboard";
import { getProperties } from "@/features/properties/api/get-properties";

export default async function DashboardPage() {
  const { getToken } = await auth();
  const token = await getToken();

  if (!token) {
    redirect("/sign-in");
  }

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
          You don't have any active purchases yet.
        </div>
      ) : (
        <PurchaseDashboard initialProperties={properties} />
      )}
    </div>
  );
}
