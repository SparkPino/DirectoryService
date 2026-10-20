"use client";

import { ChevronRight, FolderInput } from "lucide-react";
import { cn } from "@/shared/lib/utils";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/shared/ui/collapsible";
import { Button } from "@/shared/ui/button";

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
  onMove?: () => void;
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
  onMove,
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
          "group flex items-center gap-1 rounded-md py-1.5 pr-2 text-sm cursor-pointer select-none",
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

        {onMove && (
          <Button
            variant="ghost"
            size="icon"
            title="Перенести"
            aria-label="Перенести"
            className="ml-auto size-6 shrink-0 opacity-0 group-hover:opacity-100 focus-visible:opacity-100"
            onClick={(e) => {
              e.stopPropagation();
              onMove();
            }}
          >
            <FolderInput className="size-4" />
          </Button>
        )}
      </div>

      <CollapsibleContent>{children}</CollapsibleContent>
    </Collapsible>
  );
}
