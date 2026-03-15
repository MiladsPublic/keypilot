import type { Metadata } from "next";

import "@/styles/globals.css";
import { AppShell } from "@/components/layout/app-shell";
import { Providers } from "@/app/providers";

export const metadata: Metadata = {
  title: "KeyPilot",
  description: "Track the property purchase journey from offer acceptance to settlement."
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className="font-[family-name:var(--font-sans)] antialiased">
        <Providers>
          <AppShell>{children}</AppShell>
        </Providers>
      </body>
    </html>
  );
}
