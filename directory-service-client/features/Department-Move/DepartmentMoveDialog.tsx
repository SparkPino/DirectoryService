import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/ui/dialog";
import { Button } from "@/shared/ui/button";
import { Department, MovingDepartment } from "@/entities/departments/types";
import { DepartmentSelectDialog } from "../Department-Select/Department-Select-Dialog";
import { useState } from "react";
import { ArrowRight, FolderInput } from "lucide-react";
import { useDepartmentMove } from "./model/use-department-move";
import { getMoveError } from "./model/move-errors";
import { Switch } from "@/shared/ui/switch";

type DepartmentMoveDialogProps = {
  movingDepartment: MovingDepartment;
  onClose: () => void;
};

export function DepartmentMoveDialog({
  movingDepartment,
  onClose,
}: DepartmentMoveDialogProps) {
  const [newParent, setNewParent] = useState<Department | null>(null);
  const [toRoot, setToRoot] = useState(false);
  const currentParentName = movingDepartment.parent?.name ?? "Корень";
  const isAlreadyRoot = !movingDepartment.parent;
  const { mutateAsync, isPending, error, reset } = useDepartmentMove();
  const moveError = error ? getMoveError(error) : null;
  // Под select показываем только если он виден, иначе - общим блоком
  const fieldError = moveError?.isFieldError && !toRoot ? moveError.text : null;
  const operationError = moveError && !fieldError ? moveError.text : null;

  const handleParentChange = (department: Department | null) => {
    setNewParent(department);
    reset();
  };

  const handleToRootChange = (checked: boolean) => {
    setToRoot(checked);
    reset();
  };

  const hasTarget = toRoot || newParent !== null;
  const targetName = toRoot ? "Корень" : newParent?.name;

  const handler = async () => {
    try {
      await mutateAsync({
        departmentId: movingDepartment.department.id,
        parentId: toRoot ? null : (newParent?.departmentId ?? null),
      });
      onClose();
    } catch {}
  };

  return (
    <Dialog open onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Перенос подразделения</DialogTitle>
          <DialogDescription>
            Выберите новое родительское подразделение.
          </DialogDescription>
        </DialogHeader>

        <div className="min-w-0 space-y-4">
          <dl className="grid grid-cols-[auto_minmax(0,1fr)] gap-x-4 gap-y-1.5 text-sm">
            <dt className="text-muted-foreground">Подразделение</dt>
            <dd className="truncate font-medium">
              {movingDepartment.department.name}
            </dd>
            <dt className="text-muted-foreground">Текущий родитель</dt>
            <dd className="truncate">{currentParentName}</dd>
          </dl>

          <label className="flex cursor-pointer items-center justify-between gap-3 rounded-md border px-3 py-2 has-disabled:cursor-not-allowed has-disabled:opacity-50">
            <span className="text-sm">
              <span className="block font-medium">Перенести в корень</span>
              <span className="block text-xs text-muted-foreground">
                {isAlreadyRoot
                  ? "Подразделение уже находится в корне"
                  : "Подразделение станет верхнеуровневым"}
              </span>
            </span>
            <Switch
              checked={toRoot}
              onCheckedChange={handleToRootChange}
              disabled={isAlreadyRoot || isPending}
            />
          </label>

          {!toRoot && (
            <div className="space-y-1.5">
              <p className="text-sm font-medium">Новый родитель</p>
              <DepartmentSelectDialog
                mode="single"
                value={newParent}
                onChange={handleParentChange}
                excludeSubtreeOf={movingDepartment.department.id}
                className="w-full"
              />
              {fieldError && (
                <p className="text-xs text-destructive">{fieldError}</p>
              )}
            </div>
          )}

          {hasTarget && (
            <div className="flex items-center gap-2 rounded-md border bg-muted/50 px-3 py-2 text-sm">
              <span className="truncate text-muted-foreground">
                {currentParentName}
              </span>
              <ArrowRight className="size-4 shrink-0 text-muted-foreground" />
              <span className="truncate font-medium">{targetName}</span>
            </div>
          )}

          {operationError && (
            <div className="rounded-md border border-destructive/50 bg-destructive/10 px-3 py-2 text-sm text-destructive">
              {operationError}
            </div>
          )}
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            disabled={isPending}
            onClick={() => onClose()}
          >
            Отмена
          </Button>
          <Button
            disabled={!hasTarget || isPending}
            onClick={() => handler()}
          >
            <FolderInput />
            {isPending ? "Переносим..." : "Перенести"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
