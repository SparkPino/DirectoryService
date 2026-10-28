import { Spinner } from "@/shared/ui/spinner";
import PositionPage from "@/widgets/positions/position-page";
import { Suspense } from "react";

export const metadata = { title: "Позиции" };

export default function PositionsPage() {
  return (
    <Suspense
      fallback={
        <div className="flex justify-center py-8">
          <Spinner className="size-6" />
        </div>
      }
    >
      <div className="max-w-2xl mx-auto py-10 px-4">
        <h1 className="text-2xl font-semibold mb-6 text-center">Позиции</h1>
        <PositionPage />
      </div>
    </Suspense>
  );
}
