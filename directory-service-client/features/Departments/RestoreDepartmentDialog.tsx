import { Button } from "@/shared/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/shared/ui/dialog";
import { Department } from "@/entities/departments/types";
import { ArchiveRestore } from "lucide-react";
import { useRestoreDepartment } from "./model/use-department-restore";

export function RestoreDepartmentDialog({
  department,
  restoringClose,
}: {
  department: Department;
  restoringClose: () => void;
}) {
  const { mutateAsync, isPending } = useRestoreDepartment();

  const handleConfirm = async () => {
    try {
      await mutateAsync(department.departmentId, {
        onSuccess: () => restoringClose(),
      });
    } catch {}
  };

  return (
    <Dialog open onOpenChange={restoringClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Восстановление подразделения</DialogTitle>
          <DialogDescription>
            Подразделение «{department.name}» вернётся в список активных вместе
            с дочерними подразделениями, удалёнными одновременно с ним.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter className="mt-6">
          <Button
            variant="outline"
            onClick={restoringClose}
            disabled={isPending}
          >
            Отмена
          </Button>
          <Button onClick={handleConfirm} disabled={isPending}>
            <ArchiveRestore />
            {isPending ? "Восстановление..." : "Восстановить"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
