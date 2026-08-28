"use client";

import { Dispatch, SetStateAction, useState, FormEvent } from "react";
import { LocationQuery, SortDirection } from "@/entities/locations/types";
import { Input } from "@/shared/ui/input";
import { Button } from "@/shared/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";

type LocationFilterProps = {
  query: LocationQuery;
  onChange: Dispatch<SetStateAction<LocationQuery>>;
};

export default function LocationFilter({
  query,
  onChange,
}: LocationFilterProps) {
  const [prevQuery, setPrevQuery] = useState(query);
  const [search, setSearch] = useState(query.Search ?? "");
  const [minDepartmentCount, setMinDepartmentCount] = useState(
    query.MinDepartmentCount?.toString() ?? "",
  );
  const [orderBy, setOrderBy] = useState(query.OrderBy ?? "Name");
  const [sortDirection, setSortDirection] = useState<SortDirection>(
    query.SortDirection ?? "ASC",
  );

  const [pageSize, setPageSize] = useState<number>(query.PageSize ?? 10);

  if (prevQuery !== query) {
    setPrevQuery(query);
    setSearch(query.Search ?? "");
    setMinDepartmentCount(query.MinDepartmentCount?.toString() ?? "");
    setOrderBy(query.OrderBy ?? "Name");
    setSortDirection(query.SortDirection ?? "ASC");
    setPageSize(query.PageSize ?? 10);
  }

  function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    onChange((prev) => ({
      ...prev,
      Search: search || undefined,
      MinDepartmentCount: minDepartmentCount
        ? Number(minDepartmentCount)
        : undefined,
      OrderBy: orderBy,
      SortDirection: sortDirection,
      Page: 1,
      PageSize: pageSize,
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
        <label className="text-xs text-muted-foreground">
          Мин. подразделений
        </label>
        <Input
          type="number"
          min={0}
          value={minDepartmentCount}
          onChange={(e) => setMinDepartmentCount(e.target.value)}
          className="w-32"
        />
      </div>

      <div className="flex flex-col gap-1">
        <label className="text-xs text-muted-foreground">На странице</label>
        <Select
          value={pageSize.toString()}
          onValueChange={(value) => setPageSize(Number(value))}
        >
          <SelectTrigger className="w-24">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="1">1</SelectItem>
            <SelectItem value="10">10</SelectItem>
            <SelectItem value="25">25</SelectItem>
            <SelectItem value="50">50</SelectItem>
          </SelectContent>
        </Select>
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
            <SelectItem value="Name">Названию</SelectItem>
            <SelectItem value="CreatedAt">Дате создания</SelectItem>
            <SelectItem value="AttachDepartmentCount">
              Кол-ву подразделений
            </SelectItem>
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
