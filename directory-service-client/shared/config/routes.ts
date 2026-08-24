import { Home, Users, MapPin, Briefcase } from "lucide-react"

export const ROUTES = {
  home: "/",
  departments: "/departments",
  locations: "/locations",
  positions: "/positions",
} as const;


export const Navigation = [
  {
    title: "Главная",
    href: ROUTES.home,
    icon: Home,
  },
  {
    title: "Подразделения",
    href: ROUTES.departments,
    icon: Users,
  },
  {
    title: "Локации",
    href: ROUTES.locations,
    icon: MapPin,
  },
  {
    title: "Позиции",
    href: ROUTES.positions,
    icon: Briefcase,
  },
];

