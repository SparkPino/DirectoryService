import { departmentQueryOptions } from "@/entities/departments/api";
import { useQuery } from "@tanstack/react-query";

export function useDepartmentChildrenTree(
  departmentId: string,
  enabled: boolean,
) {
  const { data, error, isPending, refetch, isLoading } = useQuery({
    ...departmentQueryOptions.getDepartmentTreeChildrenOptions(departmentId),
    enabled,
  });

  return {
    data,
    error,
    isPending,
    refetch,
    isLoading,
  };
}
