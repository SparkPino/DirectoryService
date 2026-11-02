import { DepartmentBulkAction } from "@/entities/departments/types";
import { DepartmentView } from "@/entities/departments/ui/DepartmentsList";
import { Button } from "@/shared/ui/button";
import { Archive, ArchiveRestore, Power, PowerOff, X } from "lucide-react";
import { MAX_BULK_DEPARTMENTS } from "./model/bulk-limit";

type BulkActionBarProps = {
  view: DepartmentView;
  selectedCount: number;
  isLimitReached: boolean;
  disabled: boolean;
  onAction: (action: DepartmentBulkAction) => void;
  onClear: () => void;
};

export function BulkActionBar({
  view,
  selectedCount,
  isLimitReached,
  disabled,
  onAction,
  onClear,
}: BulkActionBarProps) {
  return (
    <div className="sticky bottom-4 z-10 flex flex-wrap items-center gap-3 rounded-lg bg-popover p-3 shadow-lg ring-1 ring-foreground/10">
      <div className="mr-auto flex items-center gap-1">
        <Button
          variant="ghost"
          size="icon"
          onClick={onClear}
          disabled={disabled}
          aria-label="Снять выбор"
        >
          <X />
        </Button>
        <div>
          <p className="text-sm font-medium">Выбрано: {selectedCount}</p>
          {isLimitReached && (
            <p className="text-xs text-muted-foreground">
              Это максимум за одну операцию ({MAX_BULK_DEPARTMENTS})
            </p>
          )}
        </div>
      </div>

      <div className="flex flex-wrap gap-2">
        {view === "active" ? (
          <>
            <Button
              variant="outline"
              onClick={() => onAction("activate")}
              disabled={disabled}
            >
              <Power />
              Активировать
            </Button>
            <Button
              variant="outline"
              onClick={() => onAction("deactivate")}
              disabled={disabled}
            >
              <PowerOff />
              Деактивировать
            </Button>
            <Button
              variant="destructive"
              onClick={() => onAction("soft-delete")}
              disabled={disabled}
            >
              <Archive />
              Архивировать
            </Button>
          </>
        ) : (
          <Button onClick={() => onAction("restore")} disabled={disabled}>
            <ArchiveRestore />
            Восстановить
          </Button>
        )}
      </div>
    </div>
  );
}
