import Link from "next/link";
import { notFound, redirect } from "next/navigation";
import { auth } from "@clerk/nextjs/server";

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
  const { getToken } = await auth();
  const token = await getToken();

  if (!token) {
    redirect("/sign-in");
  }

  try {
    const property = await getProperty(id, token);

    return (
      <div className="mx-auto max-w-7xl space-y-6">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">Purchase workspace</p>
            <h1 className="text-3xl font-semibold md:text-4xl">Purchase detail</h1>
          </div>
          <Link href="/properties/new" className="inline-flex items-center justify-center rounded-full bg-ink px-5 py-2.5 text-sm font-semibold text-white">
            Create purchase
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
