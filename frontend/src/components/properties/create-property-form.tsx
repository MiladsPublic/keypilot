"use client";

import { startTransition } from "react";
import { useRouter } from "next/navigation";
import { useMutation } from "@tanstack/react-query";
import { Home, LoaderCircle } from "lucide-react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { createProperty } from "@/features/properties/api/create-property";
import {
  createPropertySchema,
  type CreatePropertyFormValues
} from "@/features/properties/schemas/create-property-schema";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

export function CreatePropertyForm() {
  const router = useRouter();
  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<CreatePropertyFormValues>({
    resolver: zodResolver(createPropertySchema),
    defaultValues: {
      address: "",
      offerAcceptedDate: "",
      settlementDate: "",
      purchasePrice: ""
    }
  });

  const mutation = useMutation({
    mutationFn: createProperty,
    onSuccess: (property) => {
      startTransition(() => {
        router.push(`/properties/${property.id}`);
      });
    }
  });

  const onSubmit = handleSubmit(async (values) => {
    await mutation.mutateAsync(values);
  });

  return (
    <Card className="overflow-hidden">
      <CardHeader className="border-b border-line bg-canvas/80">
        <div className="flex items-center gap-3">
          <div className="rounded-2xl bg-accent/10 p-3 text-accent">
            <Home className="h-5 w-5" />
          </div>
          <div>
            <CardTitle>New property workspace</CardTitle>
            <CardDescription>Capture the purchase timeline first. Tasks and documents can build on top later.</CardDescription>
          </div>
        </div>
      </CardHeader>
      <CardContent className="pt-6">
        <form className="space-y-5" onSubmit={onSubmit}>
          <label className="block space-y-2">
            <span className="text-sm font-medium text-ink/80">Property address</span>
            <Input placeholder="12 Harbour View Road, Auckland" {...register("address")} />
            {errors.address ? <p className="text-sm text-red-700">{errors.address.message}</p> : null}
          </label>

          <div className="grid gap-5 md:grid-cols-2">
            <label className="block space-y-2">
              <span className="text-sm font-medium text-ink/80">Offer accepted date</span>
              <Input type="date" {...register("offerAcceptedDate")} />
              {errors.offerAcceptedDate ? (
                <p className="text-sm text-red-700">{errors.offerAcceptedDate.message}</p>
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

          <label className="block space-y-2">
            <span className="text-sm font-medium text-ink/80">Purchase price (optional)</span>
            <Input inputMode="decimal" placeholder="985000" {...register("purchasePrice")} />
            {errors.purchasePrice ? (
              <p className="text-sm text-red-700">{errors.purchasePrice.message}</p>
            ) : null}
          </label>

          {mutation.isError ? (
            <p className="rounded-2xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
              Unable to create the property. Check the backend is running on `NEXT_PUBLIC_API_BASE_URL`.
            </p>
          ) : null}

          <Button type="submit" disabled={mutation.isPending} className="w-full sm:w-auto">
            {mutation.isPending ? <LoaderCircle className="mr-2 h-4 w-4 animate-spin" /> : null}
            Create property
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
