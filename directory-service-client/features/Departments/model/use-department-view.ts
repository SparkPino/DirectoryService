import { DepartmentView } from "@/entities/departments/ui/DepartmentsList";
import { usePathname, useRouter, useSearchParams } from "next/navigation";

const PARAM = "view";
export function useDepartmentView() {
  const searchParam = useSearchParams();
  const router = useRouter();
  const pathName = usePathname();
  const raw = searchParam.get(PARAM);
  const view: DepartmentView = raw === "archived" ? "archived" : "active";

  function setDepartmentView(departmentView: DepartmentView) {
    // readonly searchParam превращает в изменяемый словарь params
    const params = new URLSearchParams(searchParam);
    params.set(PARAM, departmentView);

    const viewState = params.toString();
    const url = `${pathName}?${viewState}`;
    router.replace(url, { scroll: false });
  }

  return {
    view,
    setDepartmentView,
  };
}
