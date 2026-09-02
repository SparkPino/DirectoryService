import { GetDepartmentsQuery } from "@/entities/departments/types";
import { FilterProps } from "@/shared/api/type";
import { Button } from "@/shared/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import { Input } from "@/shared/ui/input";
import { useState } from "react";

export function DepartmentFilter({
  query,
  onChange,
}: FilterProps<GetDepartmentsQuery>) {
  const [search, setSearch] = useState(query.Search ?? "");
  const [orderBy, setOrderBy] = useState(query.SortBy ?? "name");
  const [sortDirection, setSortDirection] = useState(
    query.SortDirection ?? "ASC",
  );

  function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    onChange((prev) => ({
      ...prev,
      Search: search || undefined,
      SortBy: orderBy,
      SortDirection: sortDirection,
      Pagination: {
        ...prev.Pagination,
        Page: 1,
        PageSize: prev.Pagination?.PageSize ?? 10,
      },
    }));
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="flex flex-wrap items-end gap-3 mb-6"
    >
      <div className="flex flex-col gap-1">
        <label className="text-xs text-muted-foreground">Поиск</label>
        <Input
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Название локации..."
          className="w-48"
        />
      </div>

      <div className="flex flex-col gap-1">
        <label className="text-xs text-muted-foreground">Сортировать по</label>
        <Select
          value={orderBy}
          onValueChange={(value) => setOrderBy(value ?? "Name")}
        >
          <SelectTrigger className="w-40">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="name">Названию</SelectItem>
            <SelectItem value="created_at">Дате создания</SelectItem>
          </SelectContent>
        </Select>
      </div>

      <div className="flex flex-col gap-1">
        <label className="text-xs text-muted-foreground">Направление</label>
        <Select
          value={sortDirection}
          onValueChange={(value) =>
            setSortDirection(value === "DESC" ? "DESC" : "ASC")
          }
        >
          <SelectTrigger className="w-32">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="ASC">По возрастанию</SelectItem>
            <SelectItem value="DESC">По убыванию</SelectItem>
          </SelectContent>
        </Select>
      </div>

      <Button type="submit">Найти</Button>
    </form>
  );
}
