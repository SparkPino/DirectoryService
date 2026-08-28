"use client";

import { locationApi } from "@/entities/locations/api";
import { LocationQuery, Location } from "@/entities/locations/types";
import { LocationsList } from "@/entities/locations/ui/LocationsList";
import LocationFilter from "@/features/Locations/LocationFilter";
import { ApiRequestError } from "@/shared/api/ApiRequestError";
import { Button } from "@/shared/ui/button";
import { Spinner } from "@/shared/ui/spinner";
import axios from "axios";
import { useState, useEffect } from "react";

export default function LocationsPage() {
  const [query, setQuery] = useState<LocationQuery>({ Page: 1, PageSize: 10 });
  const [locations, setLocations] = useState<Location[]>([]);
  const [error, setError] = useState<ApiRequestError | null>(null);
  const [isLoading, setIsLoading] = useState(true);

   useEffect(() => {
    const controller = new AbortController();
    // eslint-disable-next-line react-hooks/set-state-in-effect -- нужно сразу показать индикатор загрузки перед стартом запроса
    setIsLoading(true);

    locationApi
      .getAllLocations({
        query,
        signal: controller.signal,
      })
      .then((data) => {
        setLocations(data);
        setError(null);
        setIsLoading(false);
      })
      .catch((err) => {
        if (axios.isCancel(err)) return;
        if (err instanceof ApiRequestError) {
          setIsLoading(false);
          setError(err);
        } else {
          setIsLoading(false);
          setError(new ApiRequestError("Неизвестная ошибка"));
        }
      });

    return () => {
      controller.abort();
    };
  }, [query]);

  function handleRetry() {
    setQuery((q) => ({ ...q }));
  }

  return (
    <div className="max-w-2xl mx-auto py-10 px-4">
      <h1 className="text-2xl font-semibold mb-6 text-center">Локации</h1>
      <LocationFilter query={query} onChange={setQuery} />

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

      {!isLoading && !error && locations.length === 0 && (
        <p className="text-center text-muted-foreground py-8">
          Локации не найдены
        </p>
      )}

      {!isLoading && !error && locations.length > 0 && (
        <LocationsList locations={locations} />
      )}
    </div>
  );
}
