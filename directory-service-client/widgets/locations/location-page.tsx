"use client";

import { locationApi } from "@/entities/locations/api";
import { LocationQuery, Location } from "@/entities/locations/types";
import { LocationsList } from "@/entities/locations/ui/LocationsList";
import LocationFilter from "@/features/Locations/LocationFilter";
import { Button } from "@/shared/ui/button";
import axios from "axios";
import { useState, useEffect } from "react";

export default function LocationsPage() {
  const [query, setQuery] = useState<LocationQuery>({ Page: 1, PageSize: 10 });
  const [locations, setLocations] = useState<Location[]>([]);
  const [error, setError] = useState<string | null>(null);

  // TODO: завести loading-state и включать/выключать его вокруг запроса ниже

  useEffect(() => {
    const controller = new AbortController();

    locationApi
      .getAllLocations({
        query,
        signal: controller.signal,
      })
      .then((data) => {
        setLocations(data);
        setError(null);
      })
      .catch((err) => {
        if (axios.isCancel(err)) return;
        if (err instanceof Error) {
          setError(err.message);
        } else {
          setError("Неизвестная ошибка");
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

      {/* TODO: добавить сюда проверку loading, например {loading && <p>...</p>} */}

      {error && (
        <div className="text-center py-8">
          <p className="text-destructive mb-4">{error}</p>
          <Button onClick={handleRetry}>Повторить</Button>
        </div>
      )}

      {!error && locations.length === 0 && (
        <p className="text-center text-muted-foreground py-8">
          Локаций пока нет
        </p>
      )}

      {!error && locations.length > 0 && (
        <LocationsList locations={locations} />
      )}
    </div>
  );
}
