import { GetPositionsQuery } from "@/entities/positions/types";
import { SortDirection } from "@/shared/api/type";
import { Button } from "@/shared/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import { Input } from "@/shared/ui/input";
import { Dispatch, SetStateAction, useEffect, useRef, useState } from "react";

export type FilterProps = {
  query: GetPositionsQuery;
  onChange: Dispatch<SetStateAction<GetPositionsQuery>>;
  retryTrigger?: number;
};

export function PositionFilter({
  query,
  onChange,
  retryTrigger = 0,
}: FilterProps) {
  const [prevQuery, setPrevQuery] = useState(query);
  const [search, setSearch] = useState(query.Search ?? "");
  const [orderBy, setOrderBy] = useState(query.SortBy ?? "name");
  const [sortDirection, setSortDirection] = useState<SortDirection>(
    query.SortDir ?? "ASC",
  );

  if (prevQuery !== query) {
    setPrevQuery(query);
    setSearch(query.Search ?? "");
    setOrderBy(query.SortBy ?? "name");
    setSortDirection(query.SortDir ?? "ASC");
  }

  const submitRef = useRef<() => void>(() => {});

  submitRef.current = () => {
    onChange((prev) => ({
      ...prev,
      Search: search || undefined,
      SortBy: orderBy,
      SortDir: sortDirection,
      Pagination: {
        ...prev.Pagination,
        Page: 1,
        PageSize: prev.Pagination?.PageSize ?? 10,
      },
    }));
  };

  useEffect(() => {
    if (retryTrigger === 0) return;
    submitRef.current();
  }, [retryTrigger]);

  function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    onChange((prev) => ({
      ...prev,
      Search: search || undefined,
      SortBy: orderBy,
      SortDir: sortDirection,
      Pagination: {
        ...prev.Pagination,
        Page: 1,
        PageSize: prev.Pagination?.PageSize ?? 10,
      },
    }));
  }

  const sortByLabels: Record<string, string> = {
    name: "Названию",
    created_at: "Дате создания",
  };

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
          placeholder="Название должности..."
          className="w-48"
        />
      </div>

      <div className="flex flex-col gap-1">
        <label className="text-xs text-muted-foreground">Сортировать по</label>
        <Select
          value={orderBy}
          onValueChange={(value) => setOrderBy(value ?? "name")}
        >
          <SelectTrigger className="w-40">
            <SelectValue>
              {(value: string | null) =>
                value ? sortByLabels[value] : "Сортировать по"
              }
            </SelectValue>
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="name">{sortByLabels.name}</SelectItem>
            <SelectItem value="created_at">
              {sortByLabels.created_at}
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
            <SelectValue>
              {(value: string | null) =>
                value === "ASC" ? "По возрастанию" : "По убыванию"
              }
            </SelectValue>
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
