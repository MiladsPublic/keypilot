import Link from "next/link";
import { notFound } from "next/navigation";

import { PropertySummaryCard } from "@/components/properties/property-summary-card";
import { getProperty } from "@/features/properties/api/get-property";
import { ApiError } from "@/lib/api-client";

type PropertyPageProps = {
  params: Promise<{
    id: string;
  }>;
};

export default async function PropertyPage({ params }: PropertyPageProps) {
  const { id } = await params;

  try {
    const property = await getProperty(id);

    return (
      <div className="mx-auto max-w-4xl space-y-6">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-sm font-semibold uppercase tracking-[0.22em] text-accent">Property overview</p>
            <h1 className="font-[family-name:var(--font-display)] text-4xl">Workspace created</h1>
          </div>
          <Link
            href="/properties/new"
            className="inline-flex items-center justify-center rounded-full border border-line bg-white/80 px-4 py-3 text-sm font-semibold text-ink transition hover:bg-white"
          >
            Create another property
          </Link>
        </div>
        <PropertySummaryCard property={property} />
      </div>
    );
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) {
      notFound();
    }

    throw error;
  }
}
