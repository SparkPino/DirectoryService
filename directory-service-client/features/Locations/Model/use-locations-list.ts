import { locationQueryOptions } from "@/entities/locations/api";
import { LocationQuery } from "@/entities/locations/types";
import { useQuery } from "@tanstack/react-query";

  export function useLocationsList(query: LocationQuery) {
    const { data, error, isPending, refetch } = useQuery(
    locationQueryOptions.getLocationOptions(query),
  );

  return {
    data,
    error,
    isPending,
    refetch,
  };
}