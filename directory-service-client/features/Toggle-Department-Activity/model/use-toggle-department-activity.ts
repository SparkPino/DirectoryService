import {
  departmentApi,
  departmentQueryOptions,
} from "@/entities/departments/api";
import { Department } from "@/entities/departments/types";
import { PagedResult } from "@/shared/api/type";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { InfiniteData } from "@tanstack/react-query";

type ToggleDepartmentActivityVariables = {
  departmentId: string;
  isActive: boolean;
};

const departmentsInfiniteKey = [...departmentQueryOptions.baseKey, "infinite"];

export function useToggleDepartmentActivity() {
  const activate = (departmentId: string) =>
    departmentApi.activateDepartment(departmentId);
  const deactivate = (departmentId: string) =>
    departmentApi.deactivateDepartment(departmentId);

  const queryClient = useQueryClient();

  const { mutate, error, isPending, variables } = useMutation({
    mutationFn: ({
      departmentId,
      isActive,
    }: ToggleDepartmentActivityVariables) =>
      isActive ? activate(departmentId) : deactivate(departmentId),

    onSettled: () => {
      return queryClient.invalidateQueries({
        queryKey: departmentQueryOptions.baseKey,
      });
    },

    onMutate: async ({ departmentId, isActive }) => {
      await queryClient.cancelQueries({ queryKey: departmentsInfiniteKey }); // Отменяет летящие сетевые запросы (GET-рефетчи списка)
      const previous = queryClient.getQueriesData<
        InfiniteData<PagedResult<Department>>
      >({
        queryKey: departmentsInfiniteKey,
      });
      queryClient.setQueriesData<InfiniteData<PagedResult<Department>>>(
        {
          queryKey: departmentsInfiniteKey,
        },
        (old) => {
          if (!old) return old;
          return {
            ...old,
            pages: old.pages.map((page) => ({
              ...page,
              items: page.items.map((department) =>
                department.departmentId === departmentId
                  ? { ...department, isActive: isActive }
                  : department,
              ),
            })),
          };
        },
      );

      return { previous };
    },
    onError: (error, _variables, context) => {
      context?.previous.forEach(([key, data]) => {
        queryClient.setQueryData(key, data);
      });
      toast.error(error.message);
    },
  });
  const togglingDepartmentId = isPending ? variables.departmentId : null;

  return { mutate, error, togglingDepartmentId };
}
