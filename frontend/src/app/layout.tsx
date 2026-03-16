import type { Metadata } from "next";
import { ClerkProvider } from "@clerk/nextjs";
import { Inter } from "next/font/google";

import "@/styles/globals.css";
import { AppShell } from "@/components/layout/app-shell";
import { Providers } from "@/app/providers";
import { Toaster } from "@/components/ui/toaster";

const inter = Inter({
  subsets: ["latin"],
  variable: "--font-sans"
});

export const metadata: Metadata = {
  title: "KeyPilot",
  description: "Track the property purchase journey from offer acceptance to settlement."
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <ClerkProvider>
      <html lang="en">
        <body className={`${inter.variable} font-[family-name:var(--font-sans)] antialiased`}>
          <Providers>
            <AppShell>{children}</AppShell>
            <Toaster />
          </Providers>
        </body>
      </html>
    </ClerkProvider>
  );
}
