import { PositionsTable } from "@/entities/positions/ui/PositionTabs";
import { usePositionList } from "./model/use-position-list";
import { Spinner } from "@/shared/ui/spinner";
import { Button } from "@/shared/ui/button";

export function PositionByDepartmentId({
  departmentId,
}: {
  departmentId: string;
}) {
  const {
    data,
    isPending,
    canLoadMore,
    cursorRef,
    isError,
    isFetchingNextPage,
    refetch,
  } = usePositionList({ DepartmentId: departmentId });

  return (
    <div>
      {isPending && (
        <div className="flex justify-center py-8">
          <Spinner className="size-6" />
        </div>
      )}
      {isError && !data && (
        <div className="flex flex-col items-center gap-3 py-8 text-center">
          <span className="text-sm text-muted-foreground">
            Не удалось загрузить список должностей. Попробуйте обновить
            страницу.
          </span>
          <Button onClick={() => refetch()}>Повторить попытку</Button>
        </div>
      )}
      {!isPending && data && <PositionsTable items={data.items} />}
      {canLoadMore && (
        <div ref={cursorRef} className="flex justify-center py-4">
          {isFetchingNextPage && <Spinner className="size-6" />}
        </div>
      )}
    </div>
  );
}
