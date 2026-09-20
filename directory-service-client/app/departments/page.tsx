import DepartmentPage from "@/widgets/departments/department-page";

export const metadata = { title: "Департаменты" };

export default function DepartmentsPage() {
  return (
    <div className=" max-w-2xl mx-auto py-10 px-4">
      <h1 className="text-2xl font-semibold mb-6 text-center">Подразделения</h1>
      <DepartmentPage />
      </div>
    
  );
}
