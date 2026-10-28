import { searchQueryOptions } from "@/entities/search/api";
import { SearchResultItem, SearchResultType } from "@/entities/search/types";
import { ROUTES } from "@/shared/config/routes";
import { useQuery } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { useEffect, useMemo, useState } from "react";
import { useDebounce } from "use-debounce";

const routeByType: Record<SearchResultType, string> = {
  Location: ROUTES.locations,
  Department: ROUTES.departments,
  Position: ROUTES.positions,
};

function getHref(item: SearchResultItem): string {
  const baseRoute = routeByType[item.type];
  const params = new URLSearchParams({ search: item.title, focus: item.id });
  return baseRoute + "?" + params;
}

export function useGlobalSearch() {
  const router = useRouter();
  const [open, setOpen] = useState<boolean>(false);
  const [search, setSearch] = useState<string>("");
  const isTooShort = search.trim().length < 2;
  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key.toLowerCase() === "k" && (event.metaKey || event.ctrlKey)) {
        event.preventDefault();
        setOpen((prev) => !prev);
      }
    };
    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, []);

  const [debouncedSearch] = useDebounce<string>(search.trim(), 500);
  const { data, error, isFetching, isSuccess } = useQuery(
    searchQueryOptions.getSearchItem({ Search: debouncedSearch }),
  );

  const isEmpty = !isTooShort && isSuccess && data?.length === 0;

  const groups = useMemo<Record<SearchResultType, SearchResultItem[]>>(() => {
    const items = data ?? [];
    return {
      Department: items.filter((item) => item.type === "Department"),
      Location: items.filter((item) => item.type === "Location"),
      Position: items.filter((item) => item.type === "Position"),
    };
  }, [data]);

  function select(item: SearchResultItem) {
    router.push(getHref(item));
    setSearch("");
    setOpen(false);
  }

  return {
    select,
    groups,
    error,
    isFetching,
    open,
    setOpen,
    search,
    setSearch,
    isTooShort,
    isEmpty,
  };
}
