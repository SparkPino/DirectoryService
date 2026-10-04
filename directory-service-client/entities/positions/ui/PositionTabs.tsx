import { Position } from "../types";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/ui/table";

type PositionsTableProps = {
  items: Position[];
};

export function PositionsTable({ items }: PositionsTableProps) {
  return (
    <div className="@container overflow-hidden rounded-md border">
      <Table className="table-fixed">
        <TableHeader>
          <TableRow>
            <TableHead className="truncate">Название</TableHead>
            <TableHead className="hidden w-[40%] truncate @xl:table-cell">
              Описание
            </TableHead>
            <TableHead className="hidden w-24 truncate text-right @md:table-cell">
              Отделов
            </TableHead>
            <TableHead className="w-28 truncate text-right">Создана</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {items.length ? (
            items.map((position) => (
              <TableRow key={position.id}>
                <TableCell className="truncate font-medium" title={position.name}>
                  {position.name}
                </TableCell>
                <TableCell
                  className="hidden truncate text-muted-foreground @xl:table-cell"
                  title={position.description ?? undefined}
                >
                  {position.description ?? "—"}
                </TableCell>
                <TableCell className="hidden text-right @md:table-cell">
                  {position.attachDepartmentCount}
                </TableCell>
                <TableCell className="text-right">
                  {position.createdAt.substring(0, 10)}
                </TableCell>
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell
                colSpan={4}
                className="h-24 text-center text-muted-foreground"
              >
                Позиций нет
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </div>
  );
}
