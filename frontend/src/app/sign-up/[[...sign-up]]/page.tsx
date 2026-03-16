import { SignUp } from "@clerk/nextjs";

export default function SignUpPage() {
  return (
    <div className="flex min-h-[70vh] items-center justify-center">
      <div className="w-full max-w-md space-y-4">
        <p className="text-center text-sm font-semibold uppercase tracking-[0.2em] text-ink">KeyPilot</p>
        <div className="rounded-2xl border border-line bg-white p-4">
          <SignUp />
        </div>
      </div>
    </div>
  );
}
