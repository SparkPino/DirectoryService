"use client";
import { DepartmentTreeNodesDto } from "@/entities/departments/types";
import { useDepartmentChildrenTree } from "./model/use-department-children-tree";
import { DepartmentTreeNode } from "./DepartmentTreeNode";

type TreeItem = {
  department: DepartmentTreeNodesDto;
  expandedIds: Set<string>;
  selectedId: string | null;
  onToggle: (id: string) => void;
  onSelect: (id: string) => void;
};

export function DepartmentsTreeItem(item: TreeItem) {
  const expanded = item.expandedIds.has(item.department.id);
  const { data, error, isLoading } = useDepartmentChildrenTree(
    item.department.id,
    expanded && item.department.hasChildren,
  );

  return (
    <DepartmentTreeNode
      label={item.department.name}
      hasChildren={item.department.hasChildren}
      expanded={expanded}
      onToggle={() => item.onToggle(item.department.id)}
      onSelect={() => item.onSelect(item.department.id)}
      selected={item.department.id === item.selectedId}
      depth={item.department.depth}
      isLoading={isLoading}
    >
      {error && (
        <div className="text-xs text-destructive">
          {error.allErrorsLikeString}
        </div>
      )}
      {data?.map((child) => (
        <DepartmentsTreeItem
          key={child.id}
          department={child}
          expandedIds={item.expandedIds}
          selectedId={item.selectedId}
          onToggle={item.onToggle}
          onSelect={item.onSelect}
        />
      ))}
    </DepartmentTreeNode>
  );
}
