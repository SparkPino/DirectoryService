import { ROUTES } from "@/shared/config/routes"
import { cn } from "@/shared/lib/utils"
import { buttonVariants } from "@/shared/ui/button"
import Link from "next/link"

export const metadata = {title: "Позиции"}



export default function PositionsPage() {
    return (
        <div className="max-w-2xl mx-auto py-10 px-4">
            <h1 className="text-2xl font-semibold mb-6 text-center">Позиции</h1>
             <div className="justify-center flex gap-2">
                <Link href={ROUTES.departments} className={buttonVariants({variant: "default"})}>
                    Перейти к департаментам
                </Link>
                <Link href={ROUTES.locations} className={cn(buttonVariants({ variant: "default" }), "bg-red-950 hover:bg-red-900")}>
                    Перейти к локациям
                </Link>
            </div>
        </div>
    )
}