"use client";
import { GlobalSearchView } from "./GlobalSearchView";
import { useGlobalSearch } from "./model/use-global-search";

export function GlobalSearch() {
  const {
    groups,
    error,
    isEmpty,
    isFetching,
    isTooShort,
    open,
    search,
    select,
    setOpen,
    setSearch,
  } = useGlobalSearch();
  return (
    <GlobalSearchView
      open={open}
      onOpenChange={setOpen}
      search={search}
      onSearchChange={setSearch}
      groups={groups}
      isTooShort={isTooShort}
      isEmpty={isEmpty}
      isFetching={isFetching}
      error={error}
      onSelect={select}
    />
  );
}
