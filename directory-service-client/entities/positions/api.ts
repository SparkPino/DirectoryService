import { apiClient } from "@/shared/api/axios-instance";
import { Position, GetPositionsQuery } from "./types";
import { Envelope, PagedResult } from "@/shared/api/type";
import { infiniteQueryOptions } from "@tanstack/react-query";

type GetPositionsOptions = {
  query: GetPositionsQuery;
  signal: AbortSignal;
};

export const positionApi = {
  getPositions: async ({
    query,
    signal,
  }: GetPositionsOptions): Promise<PagedResult<Position>> => {
    const response = await apiClient.get<Envelope<PagedResult<Position>>>(
      "api/positions",
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

export const positionQueryOptions = {
  baseKey: ["positions"],

  getPositionInfinityOptions: (query: GetPositionsQuery) => {
    const pageSize = query.Pagination?.PageSize ?? 10;

    return infiniteQueryOptions({
      queryKey: [
        ...positionQueryOptions.baseKey,
        "infinite",
        query.Search,
        query.SortBy,
        query.SortDir,
        pageSize,
      ],
      queryFn: ({ signal, pageParam }) =>
        positionApi.getPositions({
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

      select: (data): PagedResult<Position> => ({
        items: data.pages.flatMap((page) => page.items ?? []),
        totalCount: data.pages[0].totalCount ?? 0,
        page: data.pages[0].page ?? 1,
        pageSize: data.pages[0].pageSize ?? query.Pagination?.PageSize,
        totalPage: data.pages[0].totalPage ?? 0,
      }),
    });
  },
};
