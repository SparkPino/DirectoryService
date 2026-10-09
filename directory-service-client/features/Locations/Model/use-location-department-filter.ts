import { departmentQueryOptions } from "@/entities/departments/api";
import { useQuery } from "@tanstack/react-query";
import { usePathname, useRouter, useSearchParams } from "next/navigation";

const PARAM = "departmentIds";

export function useLocationDepartmentFilter() {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const raw = searchParams.get(PARAM);
  const departmentIds = raw ? raw.split(",").filter(Boolean) : [];

  function setDepartmentIds(ids: string[]) {
    const params = new URLSearchParams(searchParams.toString());
    if (ids.length > 0) {
      params.set(PARAM, ids.join(","));
    } else {
      params.delete(PARAM);
    }
    const qs = params.toString();
    router.replace(qs ? `${pathname}?${qs}` : pathname, { scroll: false });
  }

  const { data: selectedDepartments } = useQuery({
    ...departmentQueryOptions.getDepartmentOptions({
      DepartmentIds: departmentIds,
      Pagination: { Page: 1, PageSize: departmentIds.length },
    }),
    enabled: departmentIds.length > 0,
  });

  return {
    setDepartmentIds,
    departmentIds,
    selectedDepartments,
  };
}
