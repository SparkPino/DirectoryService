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
import { Switch } from "@/shared/ui/switch";
import { ArchiveRestore, CircleOff, FolderTree, Trash2 } from "lucide-react";
import { cn } from "@/shared/lib/utils";

export type DepartmentView = "active" | "archived";

type DepartmentListProps = PagedResult<Department> & {
  view: DepartmentView;
  focusId?: string | null;
  onEdit?: (department: Department) => void;
  onDelete?: (department: Department) => void;
  onRestore?: (department: Department) => void;
  onToggleActive?: (department: Department, isActive: boolean) => void;
  togglingDepartmentId?: string | null;
};

export function DepartmentList({
  items,
  view,
  focusId,
  onEdit,
  onDelete,
  onRestore,
  onToggleActive,
  togglingDepartmentId,
}: DepartmentListProps) {
  return (
    <EntityGrid
      items={items}
      getKey={(department) => department.departmentId.toString()}
      renderItem={(department) => {
        const isInactive = view === "active" && !department.isActive;

        return (
          <Card
            className={cn(
              "min-h-36 rounded-none ring-1 ring-foreground/15 transition-colors",
              isInactive && "bg-muted/50 ring-foreground/10",
              department.departmentId === focusId && "ring-2 ring-primary",
            )}
          >
            <CardHeader>
              <CardTitle
                className={cn(
                  "line-clamp-2",
                  isInactive && "text-muted-foreground",
                )}
              >
                {department.name}
              </CardTitle>
            </CardHeader>
            <CardContent className="mt-auto flex items-center justify-between gap-2">
              <div className="flex flex-wrap gap-2">
                {!department.path.includes(".") && (
                  <Badge variant="secondary">
                    <FolderTree />
                    Корень
                  </Badge>
                )}
                {isInactive && (
                  <Badge variant="outline" className="text-muted-foreground">
                    <CircleOff />
                    Неактивно
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
              </div>
              {view === "active" && (
                <Switch
                  checked={department.isActive}
                  onCheckedChange={(checked) =>
                    onToggleActive?.(department, checked)
                  }
                  disabled={togglingDepartmentId === department.departmentId}
                  aria-label={
                    department.isActive
                      ? "Деактивировать подразделение"
                      : "Активировать подразделение"
                  }
                />
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
                <Button
                  onClick={() => onRestore?.(department)}
                  variant="outline"
                >
                  <ArchiveRestore />
                  Восстановить
                </Button>
              )}
            </CardFooter>
          </Card>
        );
      }}
    />
  );
}
