import { Spinner } from "@/shared/ui/spinner";
import { DepartmentsTreeItem } from "./DepartmentTreeItem";
import { useDepartmentTree } from "./model/use-department-tree";
import { Button } from "@/shared/ui/button";

type DepartmentTreeState = {
  selectedId: string | null;
  expandedIds: Set<string>;
  setSelectedId: (id: string) => void;
  handleToggle: (id: string) => void;
};

export function DepartmentTree(state: DepartmentTreeState) {
  const { data, error, isPending, refetch } = useDepartmentTree();
  return (
    <div>
      {isPending && (
        <div className="flex justify-center py-8">
          <Spinner className="size-6" />
        </div>
      )}

      {!isPending && error && (
        <div className="flex flex-col items-center gap-3 py-8 text-center">
          <span className="text-sm text-muted-foreground">
            Неудалось загрузить дерево отделов. Попробуйте обновить страницу.
            Не удалось загрузить дерево отделов. Попробуйте обновить страницу.
          <Button onClick={() => refetch()}>Повторить попытку</Button>
        </div>
      )}
      {!isPending && !error && data?.length === 0 && (
        <div className="flex flex-col items-center gap-3 py-8 text-center">
          <span className="text-sm text-muted-foreground">
            Дерево отделов пустое.
          </span>
        </div>
      )}
      {!isPending &&
        !error &&
        data?.map((departmentRoot) => (
          <DepartmentsTreeItem
            key={departmentRoot.id}
            department={departmentRoot}
            expandedIds={state.expandedIds}
            onToggle={state.handleToggle}
            onSelect={state.setSelectedId}
            selectedId={state.selectedId}
          ></DepartmentsTreeItem>
        ))}
    </div>
  );
}
