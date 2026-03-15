"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Home, LayoutDashboard, PlusSquare } from "lucide-react";

import { cn } from "@/lib/utils";

const links = [
  { href: "/", label: "Home", icon: Home },
  { href: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
  { href: "/properties/new", label: "New", icon: PlusSquare }
];

export function MobileNav() {
  const pathname = usePathname();

  return (
    <nav className="fixed inset-x-4 bottom-4 z-20 rounded-full border border-line bg-white/90 p-2 shadow-panel backdrop-blur md:hidden">
      <ul className="grid grid-cols-3 gap-2">
        {links.map((link) => {
          const Icon = link.icon;
          const isActive = pathname === link.href;

          return (
            <li key={link.href}>
              <Link
                href={link.href}
                className={cn(
                  "flex items-center justify-center gap-2 rounded-full px-4 py-3 text-sm font-semibold transition",
                  isActive ? "bg-ink text-white" : "text-ink/65 hover:bg-canvas"
                )}
              >
                <Icon className="h-4 w-4" />
                <span>{link.label}</span>
              </Link>
            </li>
          );
        })}
      </ul>
    </nav>
  );
}
