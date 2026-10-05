import { departmentQueryOptions } from "@/entities/departments/api";
import { useQuery } from "@tanstack/react-query";

export function useDepartmentTree() {
  const { data, error, isPending, refetch } = useQuery(
    departmentQueryOptions.getDepartmentTreeOptions(),
  );

  return {
    data,
    error,
    isPending,
    refetch,
  };
}
