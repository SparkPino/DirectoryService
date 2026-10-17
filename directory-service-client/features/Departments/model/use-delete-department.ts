import {
  departmentApi,
  departmentQueryOptions,
} from "@/entities/departments/api";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

export function useDeleteDepartment() {
  const queryClient = useQueryClient();

  const { mutateAsync, isPending, error, isError } = useMutation({
    mutationFn: (id: string) => departmentApi.deleteDepartment(id),
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: departmentQueryOptions.baseKey,
      }),
    onSuccess: () => {
      toast.success("Подразделение перемещено в архив");
    },
    onError: (error) => toast.error(error.message),
  });

  return {
    mutateAsync,
    isPending,
    error,
    isError,
  };
}
