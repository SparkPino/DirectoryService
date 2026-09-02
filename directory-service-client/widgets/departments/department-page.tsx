"use client";
import { GetDepartmentsQuery } from "@/entities/departments/types";
import { DepartmentList } from "@/entities/departments/ui/DepartmentsList";
import { DepartmentFilter } from "@/features/Departments/DepartmentFilter";
import { Button } from "@/shared/ui/button";
import { Spinner } from "@/shared/ui/spinner";
import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import Pagination from "../Pagination/Pagination";
import { departmentQueries } from "@/entities/departments/queries";

export default function DepartmentPage() {
  const [query, setQuery] = useState<GetDepartmentsQuery>({
    Search: "",
    SortBy: "name",
    SortDirection: "ASC",
    Pagination: {
      Page: 1,
      PageSize: 10,
    },
  });

  const { data, isLoading, error, isFetching } = useQuery(
    departmentQueries.list(query),
  );

  function handleRetry() {
    setQuery((q) => ({ ...q }));
  }

  return (
    <>
      <DepartmentFilter query={query} onChange={setQuery} />

      {isLoading && (
        <div className="flex justify-center py-8">
          <Spinner className="size-6" />
        </div>
      )}

      {!isLoading && error && (
        <div className="text-center py-8">
          <p className="text-destructive mb-4">
            <span>{error.message}</span>
            <span> Type: {error.type}</span>
          </p>
          <Button onClick={handleRetry}>Повторить</Button>
        </div>
      )}

      {!isLoading && !error && data?.items.length === 0 && (
        <p className="text-center text-muted-foreground py-8">
          Подразделения не найдены
        </p>
      )}

      {!isLoading && !error && data && data.items.length > 0 && (
        <DepartmentList {...data} />
      )}
      {!isLoading && !error && data && (
        <Pagination
          page={data?.page || 1}
          totalPages={data?.totalPage / data.pageSize || 1}
          onPageChange={(p) => setQuery((q) => ({ ...q, Page: p }))}
        />
      )}
    </>
  );
}
