import {
  Department,
  DepartmentBulkAction,
} from "@/entities/departments/types";
import { ApiRequestError } from "@/shared/api/ApiRequestError";
import { Button } from "@/shared/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/ui/dialog";

type BulkConfirmDialogProps = {
  action: DepartmentBulkAction;
  departments: Department[];
  isPending: boolean;
  error: ApiRequestError | null;
  onConfirm: () => void;
  onClose: () => void;
};

type DialogText = {
  title: string;
  description: (count: number) => string;
  confirm: string;
  pending: string;
};

const dialogTexts: Record<DepartmentBulkAction, DialogText> = {
  "soft-delete": {
    title: "Архивирование подразделений",
    description: (count) =>
      `В архив будет перемещено подразделений: ${count}. Их дочерние подразделения уйдут в архив вместе с ними.`,
    confirm: "Архивировать",
    pending: "Архивирование...",
  },
  restore: {
    title: "Восстановление подразделений",
    description: (count) =>
      `Будет восстановлено подразделений: ${count}. Дочерние подразделения, удалённые одновременно с ними, тоже вернутся.`,
    confirm: "Восстановить",
    pending: "Восстановление...",
  },
  activate: {
    title: "Активация подразделений",
    description: (count) => `Будет активировано подразделений: ${count}.`,
    confirm: "Активировать",
    pending: "Активация...",
  },
  deactivate: {
    title: "Деактивация подразделений",
    description: (count) =>
      `Будет деактивировано подразделений: ${count}. Если у них есть активные дочерние подразделения, их тоже нужно выбрать.`,
    confirm: "Деактивировать",
    pending: "Деактивация...",
  },
};

export function BulkConfirmDialog({
  action,
  departments,
  isPending,
  error,
  onConfirm,
  onClose,
}: BulkConfirmDialogProps) {
  const text = dialogTexts[action];
  const namesById = new Map(
    departments.map((department) => [department.departmentId, department.name]),
  );

  return (
    <Dialog
      open
      onOpenChange={(open) => {
        if (!open && !isPending) onClose();
      }}
    >
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>{text.title}</DialogTitle>
          <DialogDescription>{text.description(departments.length)}</DialogDescription>
        </DialogHeader>

        {error && (
          <div role="alert" className="space-y-2">
            <p className="font-medium text-destructive">
              {/* status есть только у ответа сервера: без него результат неизвестен */}
              {error.status
                ? `Обработано 0 из ${departments.length}, изменения отменены:`
                : "Не удалось выполнить операцию:"}
            </p>
            <ul className="max-h-48 space-y-2 overflow-y-auto">
              {error.allErrors.map((apiError, index) => {
                const name = namesById.get(apiError.invalidField ?? "");

                return (
                  <li key={`${apiError.invalidField}-${index}`}>
                    {name && <p className="font-medium">{name}</p>}
                    <p className="text-muted-foreground">{apiError.message}</p>
                  </li>
                );
              })}
            </ul>
          </div>
        )}

        <DialogFooter className="mt-2">
          <Button variant="outline" onClick={onClose} disabled={isPending}>
            Отмена
          </Button>
          <Button
            variant={action === "soft-delete" ? "destructive" : "default"}
            onClick={onConfirm}
            disabled={isPending}
          >
            {isPending ? text.pending : text.confirm}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
