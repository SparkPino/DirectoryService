"use client";

import { ChevronRight } from "lucide-react";
import { cn } from "@/shared/lib/utils";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/shared/ui/collapsible";

type DepartmentTreeNodeProps = {
  label: string;
  hasChildren: boolean;
  expanded: boolean;
  onToggle: () => void;
  selected: boolean;
  onSelect: () => void;
  depth?: number;
  isLoading?: boolean;
  children?: React.ReactNode;
};

export function DepartmentTreeNode({
  label,
  hasChildren,
  expanded,
  onToggle,
  selected,
  onSelect,
  depth = 0,
  isLoading = false,
  children,
}: DepartmentTreeNodeProps) {
  return (
    <Collapsible open={expanded} onOpenChange={onToggle}>
      <div
        role="treeitem"
        aria-selected={selected}
        aria-expanded={hasChildren ? expanded : undefined}
        onClick={onSelect}
        style={{ paddingLeft: `${depth * 16 + 8}px` }}
        className={cn(
          "flex items-center gap-1 rounded-md py-1.5 pr-2 text-sm cursor-pointer select-none",
          "hover:bg-muted",
          selected && "bg-accent text-accent-foreground font-medium",
        )}
      >
        {hasChildren ? (
          <CollapsibleTrigger
            onClick={(e) => e.stopPropagation()}
            className="flex size-5 shrink-0 items-center justify-center rounded hover:bg-muted-foreground/10"
          >
            <ChevronRight
              className={cn(
                "size-4 text-muted-foreground transition-transform duration-150",
                expanded && "rotate-90",
              )}
            />
          </CollapsibleTrigger>
        ) : (
          <span className="size-5 shrink-0" />
        )}

        <span className="truncate">{label}</span>

        {isLoading && (
          <span className="ml-1 size-3 shrink-0 animate-spin rounded-full border-2 border-muted-foreground border-t-transparent" />
        )}
      </div>

      <CollapsibleContent>{children}</CollapsibleContent>
    </Collapsible>
  );
}
