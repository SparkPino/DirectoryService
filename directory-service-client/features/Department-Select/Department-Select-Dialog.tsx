import { useId, useRef, useState } from "react";
import { useDepartmentOptions } from "./model/use-department-options";
import { Department } from "@/entities/departments/types";
import { Button } from "@/shared/ui/button";
import { Spinner } from "@/shared/ui/spinner";
import { Badge } from "@/shared/ui/badge";
import {
  Combobox,
  ComboboxChip,
  ComboboxChips,
  ComboboxChipsInput,
  ComboboxContent,
  ComboboxEmpty,
  ComboboxInput,
  ComboboxItem,
  ComboboxList,
  ComboboxValue,
} from "@/shared/ui/combobox";

type DepartmentSelectProps =
  | {
      mode: "single";
      value: Department | null;
      onChange: (value: Department | null) => void;
      excludeIds?: string[];
    }
  | {
      mode: "multiply";
      value: Department[];
      onChange: (value: Department[]) => void;
      excludeIds?: string[];
    };

const isSameDepartment = (a: Department, b: Department) =>
  a.departmentId === b.departmentId;

export function DepartmentSelectDialog(props: DepartmentSelectProps) {
  const anchorRef = useRef<HTMLDivElement>(null);

  const [search, setSearch] = useState("");
  const [isActive, setIsActive] = useState<boolean | undefined>(undefined);

  const {
    items,
    isPending,
    isError,
    hasNextPage,
    fetchNextPage,
    isFetchingNextPage,
    refetch,
    isFetchNextPageError,
    isRefetching,
  } = useDepartmentOptions({
    search: search,
    excludeIds: props.excludeIds,
    isActive: isActive,
  });

  const isMulti = props.mode === "multiply";

  return (
    <Combobox
      items={items}
      multiple={isMulti}
      value={props.value}
      onValueChange={props.onChange as any} // звужується по mode, поки спрощує ескіз
      inputValue={search}
      onInputValueChange={setSearch}
      isItemEqualToValue={isSameDepartment}
      itemToStringLabel={(d: Department) => d.name}
      filter={null} // фільтрує бекенд
    >
      {isMulti ? (
        <ComboboxChips ref={anchorRef} className="w-96 py-1.5">
          <ComboboxValue>
            {(value) =>
              (value as Department[]).map((d) => (
                <ComboboxChip key={d.departmentId}>{d.name}</ComboboxChip>
              ))
            }
          </ComboboxValue>
          <ComboboxChipsInput
            className="min-w-48"
            placeholder="Название подразделения"
          />
        </ComboboxChips>
      ) : (
        <ComboboxInput
          className="w-96"
          placeholder="Название подразделения"
          showClear
        />
      )}

      <ComboboxContent anchor={isMulti ? anchorRef : undefined}>
        <div className="flex gap-2 border-b p-2">
          <Button className="flex-1" onClick={() => setIsActive(true)}>
            Активные
          </Button>
          <Button className="flex-1" onClick={() => setIsActive(false)}>
            Не активные
          </Button>
          <Button className="flex-1" onClick={() => setIsActive(undefined)}>
            Все
          </Button>
        </div>
        {isPending && (
          <div className="flex w-full items-center justify-center py-8">
            <Spinner className="size-6" />
          </div>
        )}
        {!isPending && isError && items.length === 0 && (
          <div className="flex flex-col items-center gap-2 py-8">
            <span className="text-sm text-muted-foreground">
              Не удалось загрузить список
            </span>
            <Button
              variant="ghost"
              disabled={isRefetching}
              onClick={() => refetch()}
            >
              {isRefetching && <Spinner className="size-4" />}
              Повторить
            </Button>
          </div>
        )}
        {!isPending && !(isError && items.length === 0) && (
          <>
            <ComboboxEmpty>Ничего не найдено</ComboboxEmpty>
            <ComboboxList>
              {(department: Department) => (
                <ComboboxItem key={department.departmentId} value={department}>
                  <span className="flex-1 truncate">{department.name}</span>
                  {department.isActive === false && (
                    <Badge variant="destructive">Неактивен</Badge>
                  )}
                </ComboboxItem>
              )}
            </ComboboxList>
          </>
        )}
        {isFetchNextPageError ? (
          <div className="flex flex-col items-center gap-2 py-4">
            <span className="text-sm text-muted-foreground">
              Не удалось загрузить ещё
            </span>
            <Button
              variant="ghost"
              disabled={isFetchingNextPage}
              onClick={() => fetchNextPage()}
            >
              {isFetchingNextPage && <Spinner className="size-4" />}
              Повторить
            </Button>
          </div>
        ) : (
          hasNextPage &&
          !isPending && (
            <div className="flex justify-center py-4">
              <Button variant="ghost" onClick={() => fetchNextPage()}>
                {isFetchingNextPage && <Spinner className="size-6" />}
                Показать ещё
              </Button>
            </div>
          )
        )}
      </ComboboxContent>
    </Combobox>
  );
}
