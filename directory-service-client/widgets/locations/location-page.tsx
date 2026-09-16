"use client";

import { LocationQuery } from "@/entities/locations/types";
import { LocationsList } from "@/entities/locations/ui/LocationsList";
import LocationFilter from "@/features/Locations/LocationFilter";
import { Button } from "@/shared/ui/button";
import { Spinner } from "@/shared/ui/spinner";
import { useState } from "react";
import Pagination from "../Pagination/Pagination";
import { useLocationsList } from "@/features/Locations/Model/use-locations-list";
import { CreateLocationDialog } from "@/features/Locations/CreateLocationDialog";

export default function LocationsPage() {
  const [query, setQuery] = useState<LocationQuery>({ Page: 1, PageSize: 10 });
  const { data, error, isPending } = useLocationsList(query);
  const [open, setOpen] = useState(false);
  const [retryTrigger, setRetryTrigger] = useState(0);

  function handleRetry() {
    setRetryTrigger((t) => t + 1);
  }

  return (
    <div className="max-w-2xl mx-auto py-10 px-4 space-y-3">
      <h1 className="text-2xl font-semibold mb-6 text-center">Локации</h1>
      <LocationFilter query={query} onChange={setQuery} retryTrigger={retryTrigger} />
      <Button onClick={() => setOpen(true)} className="mb-4">
        Создать локацию
      </Button>

      {isPending && (
        <div className="flex justify-center py-8">
          <Spinner className="size-6" />
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
          Локации не найдены
        </p>
      )}

      {!isPending && !error && data && data.items.length > 0 && (
        <LocationsList {...data} />
      )}
      {!isPending && !error && data && (
        <Pagination
          page={data?.page || 1}
          totalPages={data?.totalPage  || 1}
          onPageChange={(p) => setQuery((q) => ({ ...q, Page: p }))}
        />
      )}
     <CreateLocationDialog open={open} onClose={() => setOpen(false)} />
    </div>
  );
}
