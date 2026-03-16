import { auth } from "@clerk/nextjs/server";
import { redirect } from "next/navigation";

import { PurchaseDashboard } from "@/components/dashboard/purchase-dashboard";
import { EmptyState } from "@/components/purchase/empty-state";
import { getProperties } from "@/features/properties/api/get-properties";

export default async function DashboardPage() {
  const { getToken } = await auth();
  const token = await getToken();

  if (!token) {
    redirect("/sign-in");
  }

  const properties = await getProperties(token);

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      {properties.length === 0 ? (
        <EmptyState message="You don't have any active purchases yet." actionLabel="Create purchase" actionHref="/properties/new" />
      ) : (
        <PurchaseDashboard initialProperties={properties} />
      )}
    </div>
  );
}
