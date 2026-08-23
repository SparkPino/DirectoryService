import { ROUTES } from "@/shared/config/routes";
import { Card, CardHeader, CardTitle, CardDescription } from "@/shared/ui/card";
import Link from "next/link";
import { Metadata } from "next";

export const metadata: Metadata = {
  title: "Главная",
  description: "Главная страница приложения Directory Service",
};

const sections = [
  {
    href: ROUTES.locations,
    title: "Локации",
    description: "Города и офисы компании",
  },
  {
    href: ROUTES.departments,
    title: "Подразделения",
    description: "Организационная структура компании",
  },
  {
    href: ROUTES.positions,
    title: "Позиции",
    description: "Должности сотрудников",
  },
] as const;

export default function Home() {
  return (
    <div className="max-w-3xl mx-auto py-16 px-4 flex flex-col items-center gap-10">
      <div className="flex flex-col items-center gap-2 text-center">
        <h1 className="text-3xl font-semibold tracking-tight">Directory Service</h1>
        <p className="text-muted-foreground">
          Административная панель для управления справочником организации
        </p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 w-full">
        {sections.map((section) => (
          <Link key={section.href} href={section.href}>
            <Card className="h-full rounded-none ring-1 ring-foreground/15 transition-colors hover:bg-muted/40">
              <CardHeader>
                <CardTitle>{section.title}</CardTitle>
                <CardDescription>{section.description}</CardDescription>
              </CardHeader>
            </Card>
          </Link>
        ))}
      </div>
    </div>
  );
}