import { useState } from "react";
import { MAX_BULK_DEPARTMENTS } from "./bulk-limit";

export function useDepartmentSelection(loadedIds: string[], resetKey: string) {
  const [rawSelectedIds, setRawSelectedIds] = useState<ReadonlySet<string>>(
    new Set(),
  );

  // Выбор живёт только в рамках одного набора фильтров и вкладки
  const [prevResetKey, setPrevResetKey] = useState(resetKey);
  if (resetKey !== prevResetKey) {
    setPrevResetKey(resetKey);
    setRawSelectedIds(new Set());
  }

  // Карточка могла исчезнуть из списка после рефетча: она не должна попасть в счётчик и запрос
  const selectedIds: ReadonlySet<string> = new Set(
    loadedIds.filter((id) => rawSelectedIds.has(id)),
  );

  const selectableIds = loadedIds.slice(0, MAX_BULK_DEPARTMENTS);
  const isAllSelected =
    selectableIds.length > 0 && selectableIds.every((id) => selectedIds.has(id));
  const isSomeSelected = selectedIds.size > 0 && !isAllSelected;
  const isLimitReached = selectedIds.size >= MAX_BULK_DEPARTMENTS;

  function toggle(id: string) {
    const next = new Set(selectedIds);

    if (next.has(id)) {
      next.delete(id);
    } else if (!isLimitReached) {
      next.add(id);
    }

    setRawSelectedIds(next);
  }

  function clear() {
    setRawSelectedIds(new Set());
  }

  // Заменяет выбор, а не дополняет его, иначе можно превысить лимит
  function toggleAll() {
    setRawSelectedIds(isAllSelected ? new Set() : new Set(selectableIds));
  }

  return {
    selectedIds,
    isAllSelected,
    isSomeSelected,
    isLimitReached,
    toggle,
    toggleAll,
    clear,
  };
}
