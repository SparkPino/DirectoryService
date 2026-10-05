import { useState } from "react";

export function useDepartmentTreeState() {
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [expandedIds, setExpandedIds] = useState<Set<string>>(new Set());

  const handleToggle = (id: string) => {
    setExpandedIds((prev) => {
      const newExpand = new Set(prev);
      if (newExpand.has(id)) {
        newExpand.delete(id);
      } else {
        newExpand.add(id);
      }
      return newExpand;
    });
  };

  return { selectedId, setSelectedId, expandedIds, handleToggle };
}
