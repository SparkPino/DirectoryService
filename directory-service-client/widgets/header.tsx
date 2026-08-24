import Link from "next/link";
import { ROUTES } from "@/shared/config/routes"
 import { SidebarTrigger } from "@/shared/ui/sidebar"

export default function Header() {
  return (
    <header className="sticky top-0 z-20 flex items-center justify-between p-4 bg-gray-800 text-white">
        <SidebarTrigger/>
        <Link href={ROUTES.home} className="flex items-center gap-2">
          <span className="flex size-8 items-center justify-center rounded-md bg-white text-sm font-bold text-gray-900">
            DS
          </span>
          <h1 className="text-lg font-semibold">Directory Service</h1>
        </Link>
        </header>)
}
