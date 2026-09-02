import { Envelope, PagedResult } from "@/shared/api/type";
import { apiClient } from "@/shared/api/axios-instance";
import { LocationQuery, Location } from "./types";

type GetLocationOptions = {
  query?: LocationQuery;
  signal?: AbortSignal;
};

export const locationApi = {
  getAllLocations: async (
    { query, signal }: GetLocationOptions = {
      query: { Page: 1, PageSize: 10 },
    },
  ): Promise<PagedResult<Location>> => {
    const response = await apiClient.get<Envelope<PagedResult<Location>>>(
      "api/locations",
      {
        signal,
        params: query,
      },
    );

    return (
      response.data.result ?? {
        items: [],
        totalCount: 0,
        page: query?.Page ?? 1,
        pageSize: query?.PageSize ?? 10,
        totalPage: 0,
      }
    );
  },
};
