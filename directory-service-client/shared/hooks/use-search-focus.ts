import { useSearchParams } from "next/navigation";

// Параметры, с которыми на страницу списка приходит глобальный поиск:
// ?search=<название>&focus=<id>
export function useSearchFocus() {
  const searchParams = useSearchParams();

  return {
    search: searchParams.get("search"),
    focusId: searchParams.get("focus"),
  };
}
