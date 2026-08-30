import { apiClient } from "@/shared/api/axios-instance";
import { Department, department, GetDepartmentsQuery } from "./types";
import { Envelope, PagedResult } from "@/shared/api/type";
import { ApiError } from "next/dist/server/api-utils";
import { ApiRequestError } from "@/shared/api/ApiRequestError";
import axios from "axios";

type GetDepartmentOptions = {
  query: GetDepartmentsQuery;
  signal: AbortSignal;
};

export const departmentApi = {
  getDepartments: async ({
    query,
    signal,
  }: GetDepartmentOptions): Promise<PagedResult<Department>> => {
    try {
      const response = await apiClient.get<Envelope<PagedResult<Department>>>(
        "api/departments",
        { signal, params: query },
      );

      const envelope = response.data;
      if (envelope.isError) {
        const firstError = envelope.errorList?.[0];
        throw new ApiRequestError(
          firstError?.message || "неудалось загрузить департаменты",
          firstError?.code,
          firstError?.type,
          firstError?.invalidField,
        );
      }
      return (
        envelope.result ?? {
          items: [],
          totalCount: 0,
          page: query.Pagination.Page ?? 1,
          pageSize: query.Pagination.PageSize ?? 10,
          totalPages: 0,
        }
      );
    } catch (err) {
      if (
        axios.isAxiosError<Envelope<PagedResult<Department>>>(err) &&
        err.response?.data
      ) {
        const error = err.response.data.errorList?.[0];
        const message = error?.message;
        const code = error?.code;
        const type = error?.type;
        const invalideField = error?.invalidField;
        if (message) {
          throw new ApiRequestError(message, code, type, invalideField);
        }
      }
      throw err;
    }
  },
};
