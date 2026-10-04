"use client";
import { DepartmentTree } from "@/features/DepartmentTree/DepartmentTree";
import { useDepartmentTreeState } from "@/features/DepartmentTree/model/use-department-tree-state";
import { PositionByDepartmentId } from "@/features/Positions/PositionByDepartmentId";

export function DepartmentsTreePage() {
  const { selectedId, setSelectedId, expandedIds, handleToggle } =
    useDepartmentTreeState();

  return (
    <div className="grid gap-4 lg:grid-cols-[300px_1fr]">
      <aside className="rounded-md border p-2 lg:max-h-[calc(100vh-8rem)] lg:overflow-y-auto">
        <DepartmentTree
          selectedId={selectedId}
          expandedIds={expandedIds}
          handleToggle={handleToggle}
          setSelectedId={setSelectedId}
        />
      </aside>
      <section className="min-w-0 rounded-md border p-4">
        {!selectedId && (
          <div className="text-center text-muted-foreground">
            Выберите отдел, чтобы увидеть позиции
          </div>
        )}
        {selectedId && <PositionByDepartmentId departmentId={selectedId} />}
      </section>
    </div>
  );
}
