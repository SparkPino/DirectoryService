 "use client"
import { Navigation, ROUTES } from "@/shared/config/routes";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarHeader,} from "@/shared/ui/sidebar";
import Link from "next/link";
import { usePathname } from "next/navigation";



export default function AppSidebar() {
  const pathname = usePathname();

  return (
    <Sidebar>
      <SidebarHeader><Link href={ROUTES.home}>Главная</Link></SidebarHeader>
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupContent>
        {Navigation.filter((item) => item.href !== ROUTES.home).map((item) =>{ 
          const IsActive = item.href === pathname 
          return (
    <Link 
    key={item.href} href={item.href} 
className={`flex items-center gap-3 py-2 px-3 text-sm font-medium ${
          IsActive ? "bg-gray-400 text-foreground" : "text-muted-foreground"
        }`}>
      <item.icon className="h-4 w-4" />
      {item.title}
    </Link>
  )})}
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
      <SidebarFooter />
    </Sidebar>
  );
}
