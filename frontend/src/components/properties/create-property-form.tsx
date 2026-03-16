"use client";

import { startTransition } from "react";
import { useRouter } from "next/navigation";
import { useMutation } from "@tanstack/react-query";
import { useAuth } from "@clerk/nextjs";
import { Home, LoaderCircle } from "lucide-react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { createProperty } from "@/features/properties/api/create-property";
import {
  createPropertySchema,
  type CreatePropertyFormValues
} from "@/features/properties/schemas/create-property-schema";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { toast } from "@/hooks/use-toast";

const conditionOptions = [
  { key: "finance", label: "Finance" },
  { key: "building_report", label: "Building report" },
  { key: "lim", label: "LIM" },
  { key: "insurance", label: "Insurance" },
  { key: "solicitor_approval", label: "Solicitor approval" }
] as const;

export function CreatePropertyForm() {
  const router = useRouter();
  const { getToken } = useAuth();
  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors }
  } = useForm<CreatePropertyFormValues>({
    resolver: zodResolver(createPropertySchema),
    defaultValues: {
      address: "",
      acceptedOfferDate: "",
      settlementDate: "",
      purchasePrice: "",
      depositAmount: "",
      conditions: {
        finance: true,
        building_report: false,
        lim: false,
        insurance: false,
        solicitor_approval: false
      }
    }
  });

  const mutation = useMutation({
    mutationFn: async (values: CreatePropertyFormValues) => {
      const token = await getToken();
      return createProperty(values, token);
    },
    onSuccess: (property, values) => {
      toast({
        title: "Purchase created",
        description: `${values.address.trim()} is ready.`,
        variant: "success"
      });
      startTransition(() => {
        router.push(`/properties/${property.id}`);
      });
    },
    onError: (_error, values) => {
      const addressLabel = values.address.trim() || "this purchase";
      toast({
        title: "Couldn't create purchase",
        description: `Couldn't create ${addressLabel}. Check your details and try again.`,
        variant: "danger"
      });
    }
  });

  const onSubmit = handleSubmit(async (values) => {
    await mutation.mutateAsync(values);
  });

  const conditionsState = watch("conditions");

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center gap-3">
          <div className="rounded-xl bg-[var(--muted)] p-3 text-ink">
            <Home className="h-5 w-5" />
          </div>
          <div>
            <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">Purchase setup</p>
            <CardTitle>Create purchase workspace</CardTitle>
            <CardDescription>Start with the accepted offer, settlement date, and active conditions.</CardDescription>
          </div>
        </div>
      </CardHeader>
      <CardContent className="pt-6">
        <form className="space-y-6" onSubmit={onSubmit}>
          <section className="space-y-4">
            <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">Property</p>
          <label className="block space-y-2">
            <span className="text-sm font-medium text-ink/80">Property address</span>
            <Input placeholder="12 Harbour View Road, Auckland" {...register("address")} />
            {errors.address ? <p className="text-sm text-red-700">{errors.address.message}</p> : null}
          </label>

          <div className="grid gap-5 md:grid-cols-2">
            <label className="block space-y-2">
              <span className="text-sm font-medium text-ink/80">Purchase price (optional)</span>
              <Input inputMode="decimal" placeholder="985000" {...register("purchasePrice")} />
              {errors.purchasePrice ? (
                <p className="text-sm text-red-700">{errors.purchasePrice.message}</p>
              ) : null}
            </label>

            <label className="block space-y-2">
              <span className="text-sm font-medium text-ink/80">Deposit amount (optional)</span>
              <Input inputMode="decimal" placeholder="200000" {...register("depositAmount")} />
              {errors.depositAmount ? (
                <p className="text-sm text-red-700">{errors.depositAmount.message}</p>
              ) : null}
            </label>
          </div>
          </section>

          <section className="space-y-4">
            <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">Timeline</p>
          <div className="grid gap-5 md:grid-cols-2">
            <label className="block space-y-2">
              <span className="text-sm font-medium text-ink/80">Offer accepted date</span>
              <Input type="date" {...register("acceptedOfferDate")} />
              {errors.acceptedOfferDate ? (
                <p className="text-sm text-red-700">{errors.acceptedOfferDate.message}</p>
              ) : null}
            </label>

            <label className="block space-y-2">
              <span className="text-sm font-medium text-ink/80">Settlement date</span>
              <Input type="date" {...register("settlementDate")} />
              {errors.settlementDate ? (
                <p className="text-sm text-red-700">{errors.settlementDate.message}</p>
              ) : null}
            </label>
          </div>
          </section>

          <section className="space-y-4">
            <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">Conditions</p>
            <div className="flex flex-wrap gap-2">
              {conditionOptions.map((option) => {
                const isSelected = conditionsState?.[option.key] ?? false;

                return (
                  <Button
                    key={option.key}
                    type="button"
                    variant={isSelected ? "secondary" : "outline"}
                    className="rounded-full"
                    onClick={() => setValue(`conditions.${option.key}`, !isSelected)}
                  >
                    {option.label}
                    {isSelected ? <Badge variant="success">Selected</Badge> : null}
                  </Button>
                );
              })}
            </div>
          </section>

          {mutation.isError ? (
            <p className="rounded-2xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
              Unable to create the property. Check the backend is running on `NEXT_PUBLIC_API_BASE_URL`.
            </p>
          ) : null}

          <div className="flex flex-wrap gap-2">
            <Button type="submit" disabled={mutation.isPending} className="rounded-full px-5">
            {mutation.isPending ? <LoaderCircle className="mr-2 h-4 w-4 animate-spin" /> : null}
            Create purchase
            </Button>
            <Button type="button" variant="outline" className="rounded-lg" onClick={() => router.back()}>
              Cancel
            </Button>
          </div>
        </form>
      </CardContent>
    </Card>
  );
}
