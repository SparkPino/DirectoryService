import { apiClient } from "@/shared/api/axios-instance";
import { Department, GetDepartmentsQuery } from "./types";
import { Envelope, PagedResult } from "@/shared/api/type";

type GetDepartmentOptions = {
  query: GetDepartmentsQuery;
  signal: AbortSignal;
};

export const departmentApi = {
  getDepartments: async ({
    query,
    signal,
  }: GetDepartmentOptions): Promise<PagedResult<Department>> => {
    const response = await apiClient.get<Envelope<PagedResult<Department>>>(
      "api/departments",
      { params: query, signal },
    );

    return (
      response.data.result ?? {
        items: [],
        totalCount: 0,
        page: query.Pagination?.Page ?? 1,
        pageSize: query.Pagination?.PageSize ?? 10,
        totalPage: 0,
      }
    );
  },
};
