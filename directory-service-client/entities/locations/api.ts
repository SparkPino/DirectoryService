import { Envelope, PagedResult } from "@/shared/api/type";
import { apiClient } from "@/shared/api/axios-instance";
import {
  LocationQuery,
  Location,
  LocationDto,
  LocationUpdate,
  UpdateLocationRequest,
} from "./types";
import { queryOptions } from "@tanstack/react-query";

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

  createLocation: async (location: LocationDto): Promise<string> => {
    const response = await apiClient.post<Envelope<string>>(
      "api/locations",
      location,
    );
    return response.data.result ?? "";
  },

  updateLocation: async (
    updateLocationObject: LocationUpdate,
  ): Promise<string> => {
    const { id, query } = updateLocationObject;

    const body: UpdateLocationRequest = {
      locationName: query.name,
      adressDto: query.address,
      timeZone: query.timezone,
    };

    const response = await apiClient.patch<Envelope<string>>(
      `api/locations/${id}`,
      body,
    );

    return response.data.result ?? "";
  },

  deleteLocation: async (id: string): Promise<string> => {
    const response = await apiClient.delete<Envelope<string>>(
      `api/locations/${id}/soft-delete`,
    );

    return response.data.result ?? "";
  },
};

export const locationQueryOptions = {
  baseKey: ["locations"],

  getLocationOptions: (query: LocationQuery) => {
    return queryOptions({
      queryKey: [...locationQueryOptions.baseKey, query],
      queryFn: ({ signal }) => locationApi.getAllLocations({ query, signal }),
    });
  },
};
