import Link from "next/link";
import { Show, SignInButton, SignOutButton, SignUpButton, UserButton } from "@clerk/nextjs";

import { MobileNav } from "@/components/layout/mobile-nav";

export function AppShell({ children }: { children: React.ReactNode }) {
  return (
    <div className="mx-auto flex min-h-screen w-full max-w-6xl flex-col px-4 pb-28 pt-6 sm:px-6 md:pb-10">
      <header className="mb-8 flex items-center justify-between rounded-full border border-line bg-white/70 px-5 py-4 backdrop-blur-sm">
        <Link href="/" className="text-sm font-semibold uppercase tracking-[0.2em] text-ink/80">
          KeyPilot
        </Link>
        <div className="hidden items-center gap-4 md:flex">
          <Show when="signed-in">
            <Link href="/dashboard" className="text-sm font-medium text-ink/70">
              Dashboard
            </Link>
            <Link href="/properties/new" className="text-sm font-medium text-ink/70">
              Create property
            </Link>
            <UserButton />
            <SignOutButton>
              <button type="button" className="text-sm font-medium text-ink/70">
                Sign out
              </button>
            </SignOutButton>
          </Show>
          <Show when="signed-out">
            <SignInButton mode="modal">
              <button type="button" className="text-sm font-medium text-ink/70">
                Sign in
              </button>
            </SignInButton>
            <SignUpButton mode="modal">
              <button type="button" className="rounded-full bg-ink px-3 py-2 text-sm font-medium text-white">
                Sign up
              </button>
            </SignUpButton>
          </Show>
        </div>
      </header>

      <main className="flex-1">{children}</main>
      <MobileNav />
    </div>
  );
}
