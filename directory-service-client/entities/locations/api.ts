import axios from "axios";
import { LocationQuery, Location, Envelope, PagedResult } from "./types";
import { apiClient } from "@/shared/api/axios-instance";

type GetLocationOptions = {
  query?: LocationQuery;
  signal?: AbortSignal;
};

export const locationApi = {
  getAllLocations: async (
    { query, signal }: GetLocationOptions = {
      query: { Page: 1, PageSize: 10 },
    },
  ): Promise<Location[]> => {
    try {
      const response = await apiClient.get<Envelope<PagedResult<Location>>>(
        "api/locations",
        {
          signal,
          params: query,
        },
      );

      const envelope = response.data;

      if (envelope.isError) {
        throw new Error(
          envelope.errorList?.[0]?.message ?? "Не удалось загрузить локации",
        );
      }

      return envelope.result.items;
    } catch (err) {
      if (
        axios.isAxiosError<Envelope<PagedResult<Location>>>(err) &&
        err.response?.data
      ) {
        const message = err.response.data.errorList?.[0]?.message;
        if (message) {
          throw new Error(message);
        }
      }
      throw err;
    }
  },
};
