import { Location } from "@/entities/locations/types";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import { EntityGrid } from "@/shared/ui/entity-grid";
import { PagedResult } from "@/shared/api/type";
import { Button } from "@/shared/ui/button";
import { ArchiveRestore, Trash2 } from "lucide-react";

export type LocationView = "active" | "archived";

type LocationsListProps = PagedResult<Location> & {
  view: LocationView;
  onEdit?: (location: Location) => void;
  onDelete?: (location: Location) => void;
  onRestore?: (location: Location) => void;
};

export function LocationsList({
  items,
  view,
  onEdit,
  onDelete,
  onRestore,
}: LocationsListProps) {
  return (
    <EntityGrid
      items={items}
      getKey={(location) => location.id}
      renderItem={(location) => (
        <Card className="rounded ring-1 ring-foreground/15">
          <CardHeader>
            <CardTitle>{location.name}</CardTitle>
            <CardDescription>
              {location.address.country}, {location.address.city},{" "}
              {location.address.street}
            </CardDescription>
          </CardHeader>
          <CardContent className="flex flex-wrap gap-2">
            <Badge variant="outline">
              {location.attachDepartmentCount} подразделений
            </Badge>
            {view === "archived" && (
              <Badge variant="destructive">
                Удалено: {location.deletedAt?.substring(0, 10) ?? "—"}
              </Badge>
            )}
          </CardContent>
          <CardFooter className="justify-end gap-2 ">
            {view === "active" ? (
              <>
                <Button onClick={() => onEdit?.(location)} variant={"default"}>
                  Обновить
                </Button>
                <Button
                  onClick={() => onDelete?.(location)}
                  variant={"destructive"}
                  size="icon"
                  aria-label="Удалить"
                >
                  <Trash2 />
                </Button>
              </>
            ) : (
              <Button onClick={() => onRestore?.(location)} variant="outline">
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
