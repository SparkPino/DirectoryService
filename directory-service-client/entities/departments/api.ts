import { apiClient } from "@/shared/api/axios-instance";
import { Department, GetDepartmentsQuery } from "./types";
import { Envelope, PagedResult } from "@/shared/api/type";
import { infiniteQueryOptions, queryOptions } from "@tanstack/react-query";

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

export const departmentQueryOptions = {
  baseKey: ["departments"],

  getDepartmentOptions: (query: GetDepartmentsQuery) => {
    return queryOptions({
      queryKey: [...departmentQueryOptions.baseKey, query],
      queryFn: ({ signal }) => departmentApi.getDepartments({ query, signal }),
    });
  },

  getDepartmentInfinityOptions: (query: GetDepartmentsQuery) => {
    const pageSize = query.Pagination?.PageSize ?? 10;

    return infiniteQueryOptions({
      queryKey: [
        ...departmentQueryOptions.baseKey,
        "infinite",
        query.Search,
        query.SortBy,
        query.SortDir,
        query.IsActive,
        query.ExcludeIds,
        query.LocationIds,
        query.ParentId,
        pageSize,
      ],
      queryFn: ({ signal, pageParam }) =>
        departmentApi.getDepartments({
          query: {
            ...query,
            Pagination: { Page: pageParam, PageSize: pageSize },
          },
          signal,
        }),
      initialPageParam: 1,
      getNextPageParam: (lastPage) => {
        return lastPage.page >= lastPage.totalPage
          ? undefined
          : lastPage.page + 1;
      },

      select: (data): PagedResult<Department> => ({
        items: data.pages.flatMap((page) => page.items ?? []),
        totalCount: data.pages[0].totalCount ?? 0,
        page: data.pages[0].page ?? 1,
        pageSize: data.pages[0].pageSize ?? query.Pagination?.PageSize,
        totalPage: data.pages[0].totalPage ?? 0,
      }),
    });
  },
};
