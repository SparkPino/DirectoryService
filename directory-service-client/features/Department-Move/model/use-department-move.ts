import {
  departmentApi,
  departmentQueryOptions,
} from "@/entities/departments/api";
import { MoveDepartmentToNewParent } from "@/entities/departments/types";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

export function useDepartmentMove() {
  const queryClient = useQueryClient();
  const { mutateAsync, isPending, error, reset } = useMutation({
    mutationFn: (query: MoveDepartmentToNewParent) =>
      departmentApi.moveDepartment(query),
    onSettled: () => {
      queryClient.invalidateQueries({
        queryKey: departmentQueryOptions.baseKey,
      });
    },
    onSuccess: () => {
      toast.success("Подразделение успешно перенесено");
    },
  });

  return {
    mutateAsync,
    isPending,
    error,
    reset,
  };
}
