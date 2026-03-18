"use client";

import { startTransition, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useMutation, useQuery } from "@tanstack/react-query";
import { useAuth } from "@clerk/nextjs";
import { Home, Info, LoaderCircle } from "lucide-react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import Link from "next/link";

import { createProperty } from "@/features/properties/api/create-property";
import { getWorkspaceConfig } from "@/features/properties/api/get-workspace-config";
import {
  createPropertySchema,
  type CreatePropertyFormValues
} from "@/features/properties/schemas/create-property-schema";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { toast } from "@/hooks/use-toast";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";

const DRAFT_STORAGE_KEY = "keypilot:create-purchase-draft";

export function CreatePropertyForm() {
  const router = useRouter();
  const { getToken } = useAuth();
  const [showAuthPrompt, setShowAuthPrompt] = useState(false);

  const configQuery = useQuery({
    queryKey: ["workspace-config"],
    queryFn: getWorkspaceConfig,
    staleTime: 5 * 60 * 1000
  });

  const allConditionOptions = configQuery.data?.conditionDefaults ?? [];
  const conditionsByMethod = configQuery.data?.conditionsByMethod ?? {};
  const buyingMethodOptions = configQuery.data?.buyingMethods ?? [];
  const methodProfiles = configQuery.data?.methodProfiles ?? {};

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    reset,
    formState: { errors }
  } = useForm<CreatePropertyFormValues>({
    resolver: zodResolver(createPropertySchema),
    defaultValues: {
      address: "",
      buyingMethod: "private_sale",
      acceptedOfferDate: "",
      settlementDate: "",
      purchasePrice: "",
      depositAmount: "",
      methodReference: "",
      conditions: {
        finance: true,
        building_report: false,
        lim: false,
        insurance: false,
        solicitor_approval: false
      }
    }
  });

  useEffect(() => {
    const savedDraft = window.localStorage.getItem(DRAFT_STORAGE_KEY);

    if (!savedDraft) {
      return;
    }

    try {
      const parsedDraft = JSON.parse(savedDraft) as CreatePropertyFormValues;
      reset(parsedDraft);
    } catch {
      window.localStorage.removeItem(DRAFT_STORAGE_KEY);
    }
  }, [reset]);

  const mutation = useMutation({
    mutationFn: async (values: CreatePropertyFormValues) => {
      const token = await getToken();
      const methodConditions = configQuery.data?.conditionsByMethod?.[values.buyingMethod]
        ?? configQuery.data?.conditionDefaults ?? [];
      const offsets = Object.fromEntries(
        methodConditions.map((c) => [c.type, c.daysFromAcceptedOffer])
      );
      return createProperty(values, offsets, token);
    },
    onSuccess: (property, values) => {
      window.localStorage.removeItem(DRAFT_STORAGE_KEY);
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
        title: "Couldn't start a purchase",
        description: `Couldn't start ${addressLabel}. Check your details and try again.`,
        variant: "danger"
      });
    }
  });

  const onSubmit = handleSubmit(async (values) => {
    const token = await getToken();

    if (!token) {
      window.localStorage.setItem(DRAFT_STORAGE_KEY, JSON.stringify(values));
      setShowAuthPrompt(true);
      return;
    }

    await mutation.mutateAsync(values);
  });

  const conditionsState = watch("conditions");
  const selectedBuyingMethod = watch("buyingMethod");

  const activeProfile = methodProfiles[selectedBuyingMethod];
  const conditionOptions = conditionsByMethod[selectedBuyingMethod] ?? allConditionOptions;
  const allowedConditionTypes = new Set(conditionOptions.map((c) => c.type));
  const hasConditions = conditionOptions.length > 0;

  const dateLabel = selectedBuyingMethod === "auction" ? "Auction date"
    : selectedBuyingMethod === "tender" ? "Tender accepted date"
    : selectedBuyingMethod === "deadline" ? "Deadline accepted date"
    : "Offer accepted date";

  const referenceLabel = selectedBuyingMethod === "auction" ? "Paddle / registration number"
    : selectedBuyingMethod === "tender" ? "Tender reference"
    : selectedBuyingMethod === "deadline" ? "Reference number"
    : null;

  useEffect(() => {
    const currentConditions = conditionsState ?? {};
    let changed = false;
    const next = { ...currentConditions };

    for (const key of Object.keys(next) as Array<keyof typeof next>) {
      if (next[key] && !allowedConditionTypes.has(key)) {
        next[key] = false;
        changed = true;
      }
    }

    if (changed) {
      setValue("conditions", next);
    }
  }, [selectedBuyingMethod]); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center gap-3">
          <div className="rounded-xl bg-[var(--muted)] p-3 text-ink">
            <Home className="h-5 w-5" />
          </div>
          <div>
            <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">Purchase setup</p>
            <CardTitle>Start a purchase workspace</CardTitle>
            <CardDescription>Enter a property address to begin. Add offer and settlement dates when they&apos;re known.</CardDescription>
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
            <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">Buying method</p>
            <div className="flex flex-wrap gap-2">
              {buyingMethodOptions.map((option) => (
                <Button
                  key={option.value}
                  type="button"
                  variant={selectedBuyingMethod === option.value ? "secondary" : "outline"}
                  className="rounded-full"
                  onClick={() => setValue("buyingMethod", option.value as CreatePropertyFormValues["buyingMethod"])}
                >
                  {option.label}
                  {selectedBuyingMethod === option.value ? <Badge variant="success">Selected</Badge> : null}
                </Button>
              ))}
            </div>
            {errors.buyingMethod ? <p className="text-sm text-red-700">{errors.buyingMethod.message}</p> : null}
            {activeProfile ? (
              <div className="flex items-start gap-3 rounded-xl border border-sky-200 bg-sky-50 px-4 py-3 text-sm text-sky-900">
                <Info className="mt-0.5 h-4 w-4 shrink-0" />
                <div>
                  <p>{activeProfile.description}</p>
                  <p className="mt-1 text-sky-700">Typical timeline: ~{activeProfile.typicalSettlementDays} working days to settlement.</p>
                </div>
              </div>
            ) : null}
          </section>

          <section className="space-y-4">
            <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">Timeline</p>
          <div className="grid gap-5 md:grid-cols-2">
            <label className="block space-y-2">
              <span className="text-sm font-medium text-ink/80">{dateLabel} (optional)</span>
              <Input type="date" {...register("acceptedOfferDate")} />
              {errors.acceptedOfferDate ? (
                <p className="text-sm text-red-700">{errors.acceptedOfferDate.message}</p>
              ) : null}
            </label>

            <label className="block space-y-2">
              <span className="text-sm font-medium text-ink/80">Settlement date (optional)</span>
              <Input type="date" {...register("settlementDate")} />
              {errors.settlementDate ? (
                <p className="text-sm text-red-700">{errors.settlementDate.message}</p>
              ) : null}
            </label>
          </div>
          {referenceLabel ? (
            <label className="block space-y-2">
              <span className="text-sm font-medium text-ink/80">{referenceLabel} (optional)</span>
              <Input placeholder={referenceLabel} {...register("methodReference")} />
            </label>
          ) : null}
          </section>

          {hasConditions ? (
          <section className="space-y-4">
            <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">Conditions</p>
            <div className="flex flex-wrap gap-2">
              {conditionOptions.map((option) => {
                const key = option.type as keyof CreatePropertyFormValues["conditions"];
                const isSelected = conditionsState?.[key] ?? false;

                return (
                  <Button
                    key={option.type}
                    type="button"
                    variant={isSelected ? "secondary" : "outline"}
                    className="rounded-full"
                    onClick={() => setValue(`conditions.${key}`, !isSelected)}
                  >
                    {option.label}
                    {isSelected ? <Badge variant="success">Selected</Badge> : null}
                  </Button>
                );
              })}
            </div>
          </section>
          ) : null}

          {mutation.isError ? (
            <p className="rounded-2xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
              Unable to start the purchase. Check the backend is running on NEXT_PUBLIC_API_BASE_URL.
            </p>
          ) : null}

          <div className="flex flex-wrap gap-2">
            <Button type="submit" disabled={mutation.isPending} className="rounded-full px-5">
            {mutation.isPending ? <LoaderCircle className="mr-2 h-4 w-4 animate-spin" /> : null}
            Create
            </Button>
            <Button type="button" variant="outline" className="rounded-lg" onClick={() => router.back()}>
              Cancel
            </Button>
          </div>
        </form>

        <Dialog open={showAuthPrompt} onOpenChange={setShowAuthPrompt}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Save your purchase</DialogTitle>
              <DialogDescription>To save this purchase, sign in or create an account. Your draft is saved and will be restored after you return.</DialogDescription>
            </DialogHeader>
            <DialogFooter className="gap-2 sm:justify-start">
              <Button asChild className="rounded-full">
                <Link href="/sign-up?redirect_url=/properties/new">Sign up</Link>
              </Button>
              <Button asChild variant="outline" className="rounded-full">
                <Link href="/sign-in?redirect_url=/properties/new">Sign in</Link>
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </CardContent>
    </Card>
  );
}
