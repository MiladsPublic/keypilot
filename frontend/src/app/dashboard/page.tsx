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
      {properties.length === 0 ? (
        <div className="rounded-2xl border border-line bg-white p-8 text-sm text-ink/70">
          <p>You don't have any active purchases yet.</p>
          <Link
            href="/properties/new"
            className="mt-4 inline-flex items-center justify-center rounded-full border border-line bg-white px-4 py-2 text-sm font-semibold text-ink transition hover:bg-canvas"
          >
            Create Purchase
          </Link>
        </div>
      ) : (
        <PurchaseDashboard initialProperties={properties} />
      )}
    </div>
  );
}
