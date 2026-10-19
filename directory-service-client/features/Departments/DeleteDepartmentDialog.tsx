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
import { useDeleteDepartment } from "./model/use-delete-department";

export function DeleteDepartmentDialog({
  department,
  deletingClose,
}: {
  department: Department;
  deletingClose: () => void;
}) {
  const { mutateAsync, isPending } = useDeleteDepartment();

  const handleConfirm = async () => {
    try {
      await mutateAsync(department.departmentId, {
        onSuccess: () => deletingClose(),
      });
    } catch {}
  };

  return (
    <Dialog open onOpenChange={deletingClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Удаление подразделения</DialogTitle>
          <DialogDescription>
            Подразделение «{department.name}» будет перемещено в архив. Продолжить?
          </DialogDescription>
        </DialogHeader>
        <DialogFooter className="mt-6">
          <Button
            variant="destructive"
            disabled={isPending}
            onClick={handleConfirm}
          >
            {isPending ? "Удаление..." : "Да"}
          </Button>
          <Button onClick={deletingClose} disabled={isPending}>
            Нет
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
