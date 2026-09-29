import { departmentQueryOptions } from "@/entities/departments/api";
import { GetDepartmentsQuery } from "@/entities/departments/types";
import { useInfiniteQuery } from "@tanstack/react-query";
import { useDebounce } from "use-debounce";

export function useDepartmentOptions(params: {
  search: string;
  isActive?: boolean;
  excludeIds?: string[];
}) {
  const [debouncedSearch] = useDebounce<string>(params.search, 400);

  const query: GetDepartmentsQuery = {
    Search: debouncedSearch || undefined,
    IsActive: params.isActive,
    ExcludeIds: params.excludeIds,
    Pagination: { PageSize: 10 },
  };

  const {
    refetch,
    data,
    isPending,
    isError,
    error,
    hasNextPage,
    isFetchingNextPage,
    fetchNextPage,
    isFetchNextPageError,
    isRefetching,
  } = useInfiniteQuery(
    departmentQueryOptions.getDepartmentInfinityOptions(query),
  );

  return {
    items: data?.items ?? [],
    isPending,
    isError,
    error,
    hasNextPage,
    isFetchingNextPage,
    fetchNextPage,
    refetch,
    isFetchNextPageError,
    isRefetching,
  };
}
