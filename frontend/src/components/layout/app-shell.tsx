import Link from "next/link";

import { MobileNav } from "@/components/layout/mobile-nav";

export function AppShell({ children }: { children: React.ReactNode }) {
  return (
    <div className="mx-auto flex min-h-screen w-full max-w-6xl flex-col px-4 pb-28 pt-6 sm:px-6 md:pb-10">
      <header className="mb-8 flex items-center justify-between rounded-full border border-line bg-white/70 px-5 py-4 backdrop-blur-sm">
        <Link href="/" className="text-sm font-semibold uppercase tracking-[0.2em] text-ink/80">
          KeyPilot
        </Link>
        <Link href="/properties/new" className="hidden text-sm font-medium text-ink/70 md:inline">
          Create property
        </Link>
      </header>

      <main className="flex-1">{children}</main>
      <MobileNav />
    </div>
  );
}
