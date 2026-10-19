import { MovingDepartment } from "@/entities/departments/types";
import { useState } from "react";

export function useDepartmentTreeState() {
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [expandedIds, setExpandedIds] = useState<Set<string>>(new Set());
  const [movingDepartment, setMovingDepartment] =
    useState<MovingDepartment | null>(null);
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

  return {
    movingDepartment,
    setMovingDepartment,
    selectedId,
    setSelectedId,
    expandedIds,
    handleToggle,
  };
}
