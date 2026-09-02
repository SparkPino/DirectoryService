import { queryOptions } from "@tanstack/react-query";
import { LocationQuery } from "./types";
import { locationApi } from "./api";

export const locationQueries = {
  list: (query: LocationQuery) =>
    queryOptions({
      queryKey: ["locations", query],
      queryFn: ({ signal }) => locationApi.getAllLocations({ query, signal }),
      retry: false,
    }),
};
