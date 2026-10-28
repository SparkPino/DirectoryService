import { apiClient } from "@/shared/api/axios-instance";
import { SearchQuery, SearchResultItem } from "./types";
import { Envelope } from "@/shared/api/type";
import { queryOptions } from "@tanstack/react-query";

type SearchOptions = {
  searchQuery: SearchQuery;
  signal?: AbortSignal;
};

export const searchApi = {
  search: async ({
    searchQuery,
    signal,
  }: SearchOptions): Promise<SearchResultItem[]> => {
    const response = await apiClient.get<Envelope<SearchResultItem[]>>(
      "api/search",
      { params: searchQuery, signal },
    );

    return response.data.result ?? [];
  },
};

export const searchQueryOptions = {
  baseKey: ["search"],

  getSearchItem: (query: SearchQuery) => {
    return queryOptions({
      queryKey: [...searchQueryOptions.baseKey, query.Search, query.Limit],
      queryFn: ({ signal }) => searchApi.search({ searchQuery: query, signal }),
      enabled: query.Search.length >= 2,
    });
  },
};
