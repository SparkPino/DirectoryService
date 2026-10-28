"use client";

import { Briefcase, MapPin, SearchIcon, Users, type LucideIcon } from "lucide-react";
import { SearchResultItem, SearchResultType } from "@/entities/search/types";
import { ApiRequestError } from "@/shared/api/ApiRequestError";
import {
  Command,
  CommandDialog,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/shared/ui/command";
import { Spinner } from "@/shared/ui/spinner";

const GROUPS: { type: SearchResultType; heading: string; icon: LucideIcon }[] = [
  { type: "Location", heading: "Локации", icon: MapPin },
  { type: "Department", heading: "Подразделения", icon: Users },
  { type: "Position", heading: "Позиции", icon: Briefcase },
];

type GlobalSearchViewProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  search: string;
  onSearchChange: (value: string) => void;
  groups: Record<SearchResultType, SearchResultItem[]>;
  isTooShort: boolean;
  isEmpty: boolean;
  isFetching: boolean;
  error: ApiRequestError | null;
  onSelect: (item: SearchResultItem) => void;
};

export function GlobalSearchView({
  open,
  onOpenChange,
  search,
  onSearchChange,
  groups,
  isTooShort,
  isEmpty,
  isFetching,
  error,
  onSelect,
}: GlobalSearchViewProps) {
  return (
    <>
      <button
        type="button"
        onClick={() => onOpenChange(true)}
        className="flex h-9 w-full max-w-xs items-center gap-2 rounded-md border border-white/15 bg-white/10 px-3 text-sm text-gray-300 transition-colors hover:bg-white/15 hover:text-white"
      >
        <SearchIcon className="size-4 shrink-0" />
        <span className="flex-1 text-left">Поиск…</span>
        <kbd className="hidden rounded border border-white/20 px-1.5 font-mono text-[10px] text-gray-400 sm:inline-block">
          Ctrl K
        </kbd>
      </button>

      <CommandDialog
        open={open}
        onOpenChange={onOpenChange}
        title="Глобальный поиск"
        description="Поиск по локациям, подразделениям и позициям"
      >
        <Command shouldFilter={false}>
          <CommandInput
            value={search}
            onValueChange={onSearchChange}
            placeholder="Поиск по локациям, подразделениям, позициям…"
          />
          <CommandList>
            {isTooShort && (
              <p className="py-6 text-center text-sm text-muted-foreground">
                Введите минимум 2 символа
              </p>
            )}

            {!isTooShort && error && (
              <p className="py-6 text-center text-sm text-destructive">
                {error.message}
              </p>
            )}

            {!isTooShort && isFetching && (
              <div className="flex items-center justify-center gap-2 py-3 text-sm text-muted-foreground">
                <Spinner />
                Поиск…
              </div>
            )}

            {isEmpty && <CommandEmpty>Ничего не найдено</CommandEmpty>}

            {!isTooShort &&
              GROUPS.map(({ type, heading, icon: Icon }) => {
                const items = groups[type];
                if (items.length === 0) return null;

                return (
                  <CommandGroup key={type} heading={heading}>
                    {items.map((item) => (
                      <CommandItem
                        key={item.id}
                        value={`${item.type}-${item.id}`}
                        onSelect={() => onSelect(item)}
                      >
                        <Icon className="text-muted-foreground" />
                        <div className="flex min-w-0 flex-col">
                          <span className="truncate">{item.title}</span>
                          {item.subtitle && (
                            <span className="truncate text-xs text-muted-foreground">
                              {item.subtitle}
                            </span>
                          )}
                        </div>
                      </CommandItem>
                    ))}
                  </CommandGroup>
                );
              })}
          </CommandList>
        </Command>
      </CommandDialog>
    </>
  );
}
