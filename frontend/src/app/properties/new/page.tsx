import { CreatePropertyForm } from "@/components/properties/create-property-form";

export default function NewPropertyPage() {
  return (
    <div className="mx-auto max-w-3xl space-y-6">
      <div className="space-y-2">
        <p className="text-sm font-medium uppercase tracking-[0.18em] text-ink/65">Purchase setup</p>
        <h1 className="text-3xl font-semibold md:text-4xl">Start a purchase workspace</h1>
        <p className="text-sm text-ink/70">Start with the accepted offer, settlement date, and active conditions.</p>
      </div>
      <CreatePropertyForm />
    </div>
  );
}
