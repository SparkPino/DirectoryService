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
import { Trash2 } from "lucide-react";

type LocationsListProps = PagedResult<Location> & {
  onEdit: (location: Location) => void;
  onDelete: (location: Location) => void;
};

export function LocationsList({ items, onEdit, onDelete }: LocationsListProps) {
  return (
    <EntityGrid
      items={items}
      getKey={(location) => location.id}
      renderItem={(location) => (
        <Card className="rounded-none ring-1 ring-foreground/15 rounded">
          <CardHeader>
            <CardTitle>{location.name}</CardTitle>
            <CardDescription>
              {location.address.country}, {location.address.city},{" "}
              {location.address.street}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Badge variant="outline">
              {location.attachDepartmentCount} подразделений
            </Badge>
          </CardContent>
          <CardFooter className="justify-end gap-2 ">
            <Button onClick={() => onEdit(location)} variant={"default"}>
              Обновить
            </Button>
            <Button
              onClick={() => onDelete(location)}
              variant={"destructive"}
              size="icon"
              aria-label="Удалить"
            >
              <Trash2 />
            </Button>
          </CardFooter>
        </Card>
      )}
    />
  );
}
