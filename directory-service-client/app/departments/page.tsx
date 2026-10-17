import { Spinner } from "@/shared/ui/spinner";
import DepartmentPage from "@/widgets/departments/department-page";
import { Suspense } from "react";

export const metadata = { title: "Департаменты" };

export default function DepartmentsPage() {
  return (
    <Suspense
      fallback={
        <div className="flex justify-center py-8">
          <Spinner className="size-6" />
        </div>
      }
    >
      <div className=" max-w-2xl mx-auto py-10 px-4">
        <h1 className="text-2xl font-semibold mb-6 text-center">
          Подразделения
        </h1>
        <DepartmentPage />
      </div>
    </Suspense>
  );
}
