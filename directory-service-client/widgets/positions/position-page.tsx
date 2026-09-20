"use client";
import { GetPositionsQuery } from "@/entities/positions/types";
import { PositionList } from "@/entities/positions/ui/PositionsList";
import { PositionFilter } from "@/features/Positions/PositionFilter";
import { Button } from "@/shared/ui/button";
import { Spinner } from "@/shared/ui/spinner";
import { useState } from "react";
import { usePositionList } from "@/features/Positions/model/use-position-list";

export default function PositionPage() {
  const [retryTrigger, setRetryTrigger] = useState(0);
  const [query, setQuery] = useState<GetPositionsQuery>({
    Pagination: { Page: 1, PageSize: 10 },
  });

  const { data, error, isPending, cursorRef, isFetchingNextPage } =
    usePositionList(query);

  function handleRetry() {
    setRetryTrigger((t) => t + 1);
  }

  return (
    <div className="w-full max-w-2xl mx-auto py-10 px-4 flex flex-1 flex-col space-y-3">
      <PositionFilter
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
            Позиции не найдены
          </p>
        )}

        {!isPending && !error && data && data.items.length > 0 && (
          <PositionList {...data} />
        )}
      </div>

      <div ref={cursorRef} className="flex justify-center py-4">
        {isFetchingNextPage && <Spinner className="size-6" />}
      </div>
    </div>
  );
}
