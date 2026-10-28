import { PagedResult } from "@/shared/api/type";
import { Position } from "../types";
import { EntityGrid } from "@/shared/ui/entity-grid";
import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import { cn } from "@/shared/lib/utils";

type PositionListProps = PagedResult<Position> & {
  focusId?: string | null;
};

export function PositionList({ items, focusId }: PositionListProps) {
  return (
    <EntityGrid
      items={items}
      getKey={(position) => position.id}
      renderItem={(position) => (
        <Card
          className={cn(
            "h-36 rounded-none ring-1 ring-foreground/15",
            position.id === focusId && "ring-2 ring-primary",
          )}
        >
          <CardHeader>
            <CardTitle className="line-clamp-2">{position.name}</CardTitle>
          </CardHeader>
          <CardContent className="mt-auto flex items-center gap-2">
            <Badge variant="outline">
              Created At: {position.createdAt.substring(0, 10)}
            </Badge>
            <Badge variant="secondary">
              Отделов: {position.attachDepartmentCount}
            </Badge>
          </CardContent>
        </Card>
      )}
    />
  );
}
