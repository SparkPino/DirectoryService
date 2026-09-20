"use client";
import { GetDepartmentsQuery } from "@/entities/departments/types";
import { DepartmentList } from "@/entities/departments/ui/DepartmentsList";
import { DepartmentFilter } from "@/features/Departments/DepartmentFilter";
import { Button } from "@/shared/ui/button";
import { Spinner } from "@/shared/ui/spinner";
import { useState } from "react";
import { useDepartmentList } from "@/features/Departments/model/use-department-list";

export default function DepartmentPage() {
  const [retryTrigger, setRetryTrigger] = useState(0);
  const [query, setQuery] = useState<GetDepartmentsQuery>({
    Pagination: { Page: 1, PageSize: 10 },
  });

  const { data, error, isPending, cursorRef, isFetchingNextPage, canLoadMore } =
    useDepartmentList(query);

  function handleRetry() {
    setRetryTrigger((t) => t + 1);
  }

  return (
    <div className="w-full max-w-2xl mx-auto py-10 px-4 flex flex-1 flex-col space-y-3">
      <DepartmentFilter
        query={query}
        onChange={setQuery}
        retryTrigger={retryTrigger}
      />

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
            <Button onClick={handleRetry}>Повторить</Button>
          </div>
        )}

        {!isPending && !error && data?.items.length === 0 && (
          <p className="text-center text-muted-foreground py-8">
            Подразделения не найдены
          </p>
        )}

        {!isPending && !error && data && data.items.length > 0 && (
          <DepartmentList {...data} />
        )}
      </div>
      {canLoadMore && (
        <div ref={cursorRef} className="flex justify-center py-4">
          {isFetchingNextPage && <Spinner className="size-6" />}
        </div>
      )}
    </div>
  );
}
