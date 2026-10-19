"use client";
import { Department, GetDepartmentsQuery } from "@/entities/departments/types";
import {
  DepartmentList,
  DepartmentView,
} from "@/entities/departments/ui/DepartmentsList";
import { DepartmentFilter } from "@/features/Departments/DepartmentFilter";
import { UpdateDepartmentDialog } from "@/features/Departments/UpdateDepartmentDialog";
import { DeleteDepartmentDialog } from "@/features/Departments/DeleteDepartmentDialog";
import { Button } from "@/shared/ui/button";
import { Spinner } from "@/shared/ui/spinner";
import { Tabs, TabsList, TabsTrigger } from "@/shared/ui/tabs";
import { useState } from "react";
import { useDepartmentList } from "@/features/Departments/model/use-department-list";
import { useDepartmentView } from "@/features/Departments/model/use-department-view";
import { RestoreDepartmentDialog } from "@/features/Departments/RestoreDepartmentDialog";

export default function DepartmentPage() {
  const [query, setQuery] = useState<GetDepartmentsQuery>({
    Pagination: { Page: 1, PageSize: 10 },
  });

  const [selectedUpdateDepartment, setSelectedUpdateDepartment] =
    useState<Department | null>(null);
  const [selectedDeleteDepartment, setSelectedDeleteDepartment] =
    useState<Department | null>(null);

  const [selectedRestoreDepartment, setSelectedRestoreDepartment] =
    useState<Department | null>(null);

  const { view, setDepartmentView } = useDepartmentView();

  const activeState = view === "active";

  const {
    data,
    error,
    isPending,
    cursorRef,
    isFetchingNextPage,
    canLoadMore,
    refetch,
  } = useDepartmentList({ ...query, IsActive: activeState });

  return (
    <div className="w-full max-w-2xl mx-auto py-10 px-4 flex flex-1 flex-col space-y-3">
      <Tabs
        value={view}
        onValueChange={(value) => setDepartmentView(value as DepartmentView)}
      >
        <DepartmentFilter query={query} onChange={setQuery} />
        <TabsList>
          <TabsTrigger value="active">Активные</TabsTrigger>
          <TabsTrigger value="archived">Архивные</TabsTrigger>
        </TabsList>
      </Tabs>
      <div className="flex-1">
        {isPending && (
          <div className="flex justify-center py-8">
            <Spinner />
          </div>
        )}

        {!isPending && error && (
          <div className="text-center py-8">
            <p className="text-destructive mb-4">
              <span>{error.message}</span>
              <span> Type: {error.type}</span>
            </p>
            <Button onClick={() => refetch()}>Повторить</Button>
          </div>
        )}

        {!isPending && !error && data?.items.length === 0 && (
          <p className="text-center text-muted-foreground py-8">
            {view === "archived"
              ? "В архиве нет подразделений"
              : "Подразделения не найдены"}
          </p>
        )}

        {!isPending && !error && data && data.items.length > 0 && (
          <DepartmentList
            {...data}
            view={view}
            onEdit={setSelectedUpdateDepartment}
            onDelete={setSelectedDeleteDepartment}
            onRestore={setSelectedRestoreDepartment}
          />
        )}
      </div>
      {canLoadMore && (
        <div ref={cursorRef} className="flex justify-center py-4">
          {isFetchingNextPage && <Spinner className="size-6" />}
        </div>
      )}

      {selectedUpdateDepartment && (
        <UpdateDepartmentDialog
          department={selectedUpdateDepartment}
          editingClose={() => setSelectedUpdateDepartment(null)}
        />
      )}
      {selectedDeleteDepartment && (
        <DeleteDepartmentDialog
          department={selectedDeleteDepartment}
          deletingClose={() => setSelectedDeleteDepartment(null)}
        />
      )}

      {selectedRestoreDepartment && (
        <RestoreDepartmentDialog
          department={selectedRestoreDepartment}
          restoringClose={() => setSelectedRestoreDepartment(null)}
        />
      )}
    </div>
  );
}
