import {
  departmentApi,
  departmentQueryOptions,
} from "@/entities/departments/api";
import {
  DepartmentBulkAction,
  DepartmentBulkRequest,
} from "@/entities/departments/types";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

const successMessages: Record<DepartmentBulkAction, string> = {
  "soft-delete": "Архивировано подразделений",
  restore: "Восстановлено подразделений",
  activate: "Активировано подразделений",
  deactivate: "Деактивировано подразделений",
};

export function useDepartmentBulkAction() {
  const queryClient = useQueryClient();

  const { mutate, isPending, error, reset } = useMutation({
    mutationFn: (request: DepartmentBulkRequest) =>
      departmentApi.bulkAction(request),

    onSuccess: (_data, { action, departmentIds }) => {
      toast.success(`${successMessages[action]}: ${departmentIds.length}`);
    },

    // Возвращаем промис, чтобы isPending держался до обновления списка
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: departmentQueryOptions.baseKey,
      }),
  });

  return { mutate, isPending, error, reset };
}
