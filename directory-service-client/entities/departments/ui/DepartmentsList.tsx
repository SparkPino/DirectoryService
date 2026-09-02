import { PagedResult } from "@/shared/api/type";
import { Department } from "../types";
import { EntityGrid } from "@/shared/ui/entity-grid";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";

export function DepartmentList({ items }: PagedResult<Department>) {
  return (
    <EntityGrid
      items={items}
      getKey={(department) => department.departmentId.toString()}
      renderItem={(department) => (
        <Card className="rounded-none ring-1 ring-foreground/15">
          <CardHeader>
            <CardTitle>{department.name}</CardTitle>
            <CardDescription></CardDescription>
          </CardHeader>
          <CardContent>
            <Badge variant="outline">
              Created At: {department.createdAt.substring(0, 10)}
            </Badge>
          </CardContent>
        </Card>
      )}
    />
  );
}
