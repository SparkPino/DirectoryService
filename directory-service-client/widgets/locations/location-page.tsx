"use client";

import { LocationQuery } from "@/entities/locations/types";
import { LocationsList } from "@/entities/locations/ui/LocationsList";
import LocationFilter from "@/features/Locations/LocationFilter";
import { Button } from "@/shared/ui/button";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/shared/ui/dialog";
import { Spinner } from "@/shared/ui/spinner";
import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import Pagination from "../Pagination/Pagination";
import { locationQueries } from "@/entities/locations/queries";
import { Field, FieldGroup } from "@/shared/ui/field";
import { Label } from "@/shared/ui/label";
import { Input } from "@/shared/ui/input";

export default function LocationsPage() {
  const [query, setQuery] = useState<LocationQuery>({ Page: 1, PageSize: 10 });
  const { data, error, isLoading, isFetched } = useQuery(
    locationQueries.list(query),
  );
  const [open, setOpen] = useState(false);
  function handleRetry() {
    setQuery((q) => ({ ...q }));
  }

  return (
    <div className="max-w-2xl mx-auto py-10 px-4">
      <h1 className="text-2xl font-semibold mb-6 text-center">Локации</h1>
      <LocationFilter query={query} onChange={setQuery} />
      <Button onClick={() => setOpen(true)} className="mb-4">
        Создать локацию
      </Button>

      {isLoading && (
        <div className="flex justify-center py-8">
          <Spinner className="size-6" />
        </div>
      )}

      {!isLoading && error && (
        <div className="text-center py-8">
          <p className="text-destructive mb-4">
            <span>{error.message}</span>
            <span> Type: {error.type}</span>
          </p>
          <Button onClick={handleRetry}>Повторить</Button>
        </div>
      )}

      {!isLoading && !error && data?.items.length === 0 && (
        <p className="text-center text-muted-foreground py-8">
          Локации не найдены
        </p>
      )}

      {!isLoading && !error && data && data.items.length > 0 && (
        <LocationsList {...data} />
      )}
      {!isLoading && !error && data && (
        <Pagination
          page={data?.page || 1}
          totalPages={data?.totalPage || 1}
          onPageChange={(p) => setQuery((q) => ({ ...q, Page: p }))}
        />
      )}
      <Dialog open={open} onOpenChange={setOpen}>
        <form>
          <DialogTrigger />
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Создать локацию</DialogTitle>
              <DialogDescription>
                Введите название локации и нажмите (Сохранить)
              </DialogDescription>
            </DialogHeader>

            <FieldGroup>
              <Field>
                <Label htmlFor="name-1">Название</Label>
                <Input
                  id="name-1"
                  name="name"
                  placeholder="Введите название локации"
                />
              </Field>
              <Field>
                <Label htmlFor="name-1">Название</Label>
                <Input
                  id="name-1"
                  name="name"
                  placeholder="Введите название локации"
                />
              </Field>
            </FieldGroup>
            <DialogFooter>
              <DialogClose render={<Button variant="outline">{}</Button>} />
              <Button type="submit"></Button>
            </DialogFooter>
          </DialogContent>
        </form>
      </Dialog>
    </div>
  );
}
