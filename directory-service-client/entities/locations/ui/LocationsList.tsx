import { Location } from "@/entities/locations/types";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";

type LocationsListProps = {
  locations: Location[];
};

export function LocationsList({ locations }: LocationsListProps) {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
      {locations.map((location) => (
        <Card
          key={location.id}
          className="rounded-none ring-1 ring-foreground/15"
        >
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
      ))}
    </div>
  );
}
