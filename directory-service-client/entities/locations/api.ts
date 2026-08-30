import axios from "axios";
import { LocationQuery, Location, Envelope, PagedResult } from "./types";
import { apiClient } from "@/shared/api/axios-instance";
import { ApiRequestError } from "@/shared/api/ApiRequestError";

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
        const firstError = envelope.errorList?.[0];
        throw new ApiRequestError(
          firstError?.message ?? "Не удалось загрузить локации",
          firstError?.code,
          firstError?.type,
        );
      }

      return (
        envelope.result ?? {
          items: [],
          totalCount: 0,
          page: query?.Page ?? 1,
          pageSize: query?.PageSize ?? 10,
          totalPages: 0,
        }
      );
    } catch (err) {
      if (
        axios.isAxiosError<Envelope<PagedResult<Location>>>(err) &&
        err.response?.data
      ) {
        const message = err.response.data.errorList?.[0]?.message;
        const code = err.response.data.errorList?.[0]?.code;
        const type = err.response.data.errorList?.[0]?.type;
        if (message) {
          throw new ApiRequestError(message, code, type);
        }
      }
      throw err;
    }
  },
};
