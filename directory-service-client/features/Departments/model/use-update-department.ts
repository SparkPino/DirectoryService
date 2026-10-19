import {
  departmentApi,
  departmentQueryOptions,
} from "@/entities/departments/api";
import { DepartmentUpdate } from "@/entities/departments/types";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

export function useUpdateDepartment() {
  const queryClient = useQueryClient();

  const { mutateAsync, isPending, error, isError } = useMutation({
    mutationFn: (update: DepartmentUpdate) =>
      departmentApi.updateDepartment(update),
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: departmentQueryOptions.baseKey,
      }),
    onSuccess: () => {
      toast.success("Подразделение успешно обновлено");
    },
  });

  return {
    mutateAsync,
    isPending,
    error,
    isError,
  };
}
