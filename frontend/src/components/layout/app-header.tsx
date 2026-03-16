"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { UserButton, useUser } from "@clerk/nextjs";
import { ChevronDown, LayoutDashboard, PlusCircle } from "lucide-react";

import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger
} from "@/components/ui/dropdown-menu";
import { Separator } from "@/components/ui/separator";

const links = [
  { href: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
  { href: "/properties/new", label: "Start a purchase", icon: PlusCircle }
];

export function AppHeader() {
  const pathname = usePathname();
  const { isSignedIn } = useUser();

  return (
    <header className="mb-6 rounded-2xl border border-line bg-white px-4 py-3 sm:px-6">
      <div className="flex items-center justify-between gap-4">
        <Link href="/" className="text-sm font-semibold uppercase tracking-[0.2em] text-ink">
          KeyPilot
        </Link>

        {isSignedIn ? (
          <>
            <nav className="hidden items-center gap-1 md:flex">
              {links.map((link) => {
                const Icon = link.icon;
                const isActive = pathname === link.href;

                return (
                  <Button key={link.href} asChild variant={isActive ? "secondary" : "ghost"} size="sm" className="rounded-lg">
                    <Link href={link.href}>
                      <Icon className="h-4 w-4" />
                      {link.label}
                    </Link>
                  </Button>
                );
              })}
            </nav>
            <div className="flex items-center gap-3">
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant="ghost" size="sm" className="hidden rounded-lg md:inline-flex">
                    Menu
                    <ChevronDown className="h-4 w-4" />
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end">
                  {links.map((link) => (
                    <DropdownMenuItem key={link.href} asChild>
                      <Link href={link.href}>{link.label}</Link>
                    </DropdownMenuItem>
                  ))}
                  <DropdownMenuSeparator />
                  <DropdownMenuItem asChild>
                    <Link href="/">Home</Link>
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
              <UserButton />
            </div>
          </>
        ) : (
          <div className="flex items-center gap-2">
            <Button asChild variant="ghost" size="sm" className="rounded-lg">
              <Link href="/sign-in">Sign in</Link>
            </Button>
            <Button asChild size="sm" className="rounded-full px-4">
              <Link href="/properties/new">Start a purchase</Link>
            </Button>
          </div>
        )}
      </div>
      {isSignedIn ? (
        <>
          <Separator className="my-3" />
          <nav className="grid grid-cols-2 gap-2 md:hidden">
            {links.map((link) => {
              const Icon = link.icon;
              const isActive = pathname === link.href;
              return (
                <Button
                  key={link.href}
                  asChild
                  variant={isActive ? "secondary" : "ghost"}
                  size="sm"
                  className={cn("justify-start rounded-lg", isActive && "font-semibold")}
                >
                  <Link href={link.href}>
                    <Icon className="h-4 w-4" />
                    {link.label}
                  </Link>
                </Button>
              );
            })}
          </nav>
        </>
      ) : null}
    </header>
  );
}
