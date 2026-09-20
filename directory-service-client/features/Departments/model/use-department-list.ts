import { departmentQueryOptions } from "@/entities/departments/api";
import { GetDepartmentsQuery } from "@/entities/departments/types";
import { useInfiniteQuery } from "@tanstack/react-query";
import { RefCallback, useCallback } from "react";

export function useDepartmentList(query: GetDepartmentsQuery) {
  const {
    data,
    isPending,
    error,
    isError,
    refetch,
    fetchNextPage,
    isFetchingNextPage,
    hasNextPage,
  } = useInfiniteQuery({
    ...departmentQueryOptions.getDepartmentInfinityOptions(query),
  });

  //Cursor
  const cursorRef: RefCallback<HTMLDivElement> = useCallback(
    (el) => {
      if (!el) return;

      const observer = new IntersectionObserver(
        (entries) => {
          if (entries[0].isIntersecting && hasNextPage && !isFetchingNextPage) {
            fetchNextPage();
          }
        },
        { threshold: 0.5 },
      );

      observer.observe(el);

      return () => observer.disconnect();
    },
    [fetchNextPage, hasNextPage, isFetchingNextPage],
  );

  const canLoadMore = hasNextPage && !isError;
  // у useInfiniteQuery другая форма data,для useInfiniteQuery
  // data — это не то, что вернул queryFn напрямую. React Query оборачивает результат в специальную структуру:
  // pages: PagedResult<Department>[],  // результат queryFn с КАЖДОЙ загруженной страницы, по одному элементу на страницу
  // pageParams: number[]               // pageParam, с которым была вызвана каждая из этих страниц
  // React Query намеренно не пытается сам их слить в один плоский список — потому что способ слияния специфичен для твоего приложения
  // ак что для рендера списка тебе нужно будет самому "расплющить" все страницы, обычно через:
  // const items = data?.pages.flatMap((page) => page.items) ?? [];
  // а метаданные вроде totalCount или totalPage, если понадобятся — брать, например, с последней страницы: data?.pages.at(-1)?.totalCount.
  return {
    data,
    isPending,
    error,
    isError,
    refetch,
    cursorRef,
    isFetchingNextPage,
    canLoadMore,
  };
}
