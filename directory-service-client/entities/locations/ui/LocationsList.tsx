import { Location } from "@/entities/locations/types";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import { EntityGrid } from "@/shared/ui/entity-grid";
import { PagedResult } from "@/shared/api/type";

export function LocationsList({ items }: PagedResult<Location>) {
  return (
    <EntityGrid
      items={items}
      getKey={(location) => location.id}
      renderItem={(location) => (
        <Card className="rounded-none ring-1 ring-foreground/15">
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
        </Card>
      )}
    />
  );
}
