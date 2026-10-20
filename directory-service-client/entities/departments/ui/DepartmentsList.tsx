import { PagedResult } from "@/shared/api/type";
import { Department } from "../types";
import { EntityGrid } from "@/shared/ui/entity-grid";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import { Button } from "@/shared/ui/button";
import { ArchiveRestore, FolderTree, Trash2 } from "lucide-react";

export type DepartmentView = "active" | "archived";

type DepartmentListProps = PagedResult<Department> & {
  view: DepartmentView;
  onEdit?: (department: Department) => void;
  onDelete?: (department: Department) => void;
  onRestore?: (department: Department) => void;
};

export function DepartmentList({
  items,
  view,
  onEdit,
  onDelete,
  onRestore,
}: DepartmentListProps) {
  return (
    <EntityGrid
      items={items}
      getKey={(department) => department.departmentId.toString()}
      renderItem={(department) => (
        <Card className="min-h-36 rounded-none ring-1 ring-foreground/15">
          <CardHeader>
            <CardTitle className="line-clamp-2">{department.name}</CardTitle>
          </CardHeader>
          <CardContent className="mt-auto flex flex-wrap gap-2">
            {/* корень - путь из одного сегмента, без точек */}
            {!department.path.includes(".") && (
              <Badge variant="secondary">
                <FolderTree />
                Корень
              </Badge>
            )}
            <Badge variant="outline">
              Создано: {department.createdAt.substring(0, 10)}
            </Badge>
            {view === "archived" && (
              <Badge variant="destructive">
                Удалено: {department.deletedAt?.substring(0, 10) ?? "—"}
              </Badge>
            )}
          </CardContent>
          <CardFooter className="justify-end gap-2">
            {view === "active" ? (
              <>
                <Button onClick={() => onEdit?.(department)}>Обновить</Button>
                <Button
                  onClick={() => onDelete?.(department)}
                  variant="destructive"
                  size="icon"
                  aria-label="Удалить"
                >
                  <Trash2 />
                </Button>
              </>
            ) : (
              <Button onClick={() => onRestore?.(department)} variant="outline">
                <ArchiveRestore />
                Восстановить
              </Button>
            )}
          </CardFooter>
        </Card>
      )}
    />
  );
}
