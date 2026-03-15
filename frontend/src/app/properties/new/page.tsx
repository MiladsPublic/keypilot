import { CreatePropertyForm } from "@/components/properties/create-property-form";

export default function NewPropertyPage() {
  return (
    <div className="mx-auto max-w-3xl space-y-4">
      <div className="space-y-2">
        <p className="text-sm font-semibold uppercase tracking-[0.22em] text-accent">Property setup</p>
        <h1 className="font-[family-name:var(--font-display)] text-4xl">Create the first workspace</h1>
        <p className="text-ink/68">This is the first vertical slice through KeyPilot: create a property, then load its details page.</p>
      </div>
      <CreatePropertyForm />
    </div>
  );
}
