"use client";

import { LocationQuery, Location } from "@/entities/locations/types";
import {
  LocationsList,
  LocationView,
} from "@/entities/locations/ui/LocationsList";
import LocationFilter from "@/features/Locations/LocationFilter";
import { Button } from "@/shared/ui/button";
import { Spinner } from "@/shared/ui/spinner";
import { useState } from "react";
import Pagination from "../Pagination/Pagination";
import { useLocationsList } from "@/features/Locations/Model/use-locations-list";
import { CreateLocationDialog } from "@/features/Locations/CreateLocationDialog";
import { UpdateLocationDialog } from "@/features/Locations/UpdateLocation-dialog";
import { DeleteLocationDialog } from "@/features/Locations/DeleteLocationDialog";
import { RestoreLocationDialog } from "@/features/Locations/RestoreLocationDialog";
import { useLocationView } from "@/features/Locations/Model/use-location-view";
import { Tabs, TabsList, TabsTrigger } from "@/shared/ui/tabs";
import { DepartmentSelectDialog } from "@/features/Department-Select/Department-Select-Dialog";
import { Department } from "@/entities/departments/types";
import { useLocationDepartmentFilter } from "@/features/Locations/Model/use-location-department-filter";
export default function LocationsPage() {
  const [query, setQuery] = useState<LocationQuery>({ Page: 1, PageSize: 10 });
  const [open, setOpen] = useState(false);
  const [retryTrigger, setRetryTrigger] = useState(0);

  const [userSelection, setUserSelection] = useState<Department[] | null>(null);

  const { departmentIds, setDepartmentIds, selectedDepartments } =
    useLocationDepartmentFilter();
  const selectValue = userSelection ?? selectedDepartments?.items ?? [];
  const isActiveDepartmentFilter = departmentIds.length > 0;
  const { view, setLocationView } = useLocationView();
  const { data, error, isPending } = useLocationsList({
    ...query,
    DepartmentIds: isActiveDepartmentFilter ? departmentIds : undefined,
    IsActive: view === "active",
  });
  const [selectedUpdateLocation, setSelectedLocation] =
    useState<Location | null>(null);

  const [selectedDeleteLocation, setSelectedDeleteLocation] =
    useState<Location | null>(null);

  const [selectedRestoreLocation, setSelectedRestoreLocation] =
    useState<Location | null>(null);

  function onEdit(location: Location) {
    setSelectedLocation(location);
  }

  function handleDepartmentsChange(departments: Department[]) {
    setUserSelection(departments);
    setQuery((q) => ({ ...q, Page: 1 }));
    const ids = departments.map((d) => d.departmentId);
    setDepartmentIds(ids);
  }

  function handleViewChange(nextView: LocationView) {
    setLocationView(nextView);
    setQuery((q) => ({ ...q, Page: 1 }));
  }

  function handleResetFilter() {
    handleDepartmentsChange([]);
  }

  function handleItemLeftPage() {
    if (query.Page && query.Page > 1 && data?.items.length === 1) {
      setQuery((q) => ({ ...q, Page: (q.Page ?? 1) - 1 }));
    }
  }

  function onDelete(location: Location) {
    setSelectedDeleteLocation(location);
  }

  function handleRetry() {
    setRetryTrigger((t) => t + 1);
  }

  return (
    <div className="max-w-2xl mx-auto py-10 px-4 space-y-3">
      <h1 className="text-2xl font-semibold mb-6 text-center">Локации</h1>
      <div className="flex flex-wrap items-end gap-3">
        <div className="flex flex-col gap-1">
          <label className="text-xs text-muted-foreground">
            Подразделения
          </label>
          <DepartmentSelectDialog
            mode="multiply"
            value={selectValue}
            onChange={handleDepartmentsChange}
          />
        </div>
        {isActiveDepartmentFilter && (
          <Button variant="outline" onClick={handleResetFilter}>
            Сбросить фильтр
          </Button>
        )}
      </div>
      <LocationFilter
        query={query}
        onChange={setQuery}
        retryTrigger={retryTrigger}
      />

      <Button onClick={() => setOpen(true)} className="mb-4">
        Создать локацию
      </Button>
      <Tabs
        value={view}
        onValueChange={(value) => handleViewChange(value as LocationView)}
      >
        <TabsList>
          <TabsTrigger value="active">Активные</TabsTrigger>
          <TabsTrigger value="archived">Архивные</TabsTrigger>
        </TabsList>
      </Tabs>
      {isPending && (
        <div className="flex justify-center py-8">
          <Spinner className="size-6" />
        </div>
      )}
      {!isPending && error && (
        <div className="text-center py-8">
          <p className="text-destructive mb-4">
            <span>{error.message}</span>
            <span> Type: {error.type}</span>
          </p>
          <Button onClick={handleRetry}>Повторить</Button>
        </div>
      )}
      {!isPending &&
        !error &&
        data?.totalCount === 0 &&
        isActiveDepartmentFilter && (
          <div className="flex flex-col items-center gap-3 py-8 text-center">
            <span>Нет локаций для выбранных подразделений</span>
            <Button variant="outline" onClick={handleResetFilter}>
              Сбросить фильтр
            </Button>
          </div>
        )}
      {!isPending &&
        !error &&
        data?.totalCount === 0 &&
        !isActiveDepartmentFilter && (
          <p className="text-center text-muted-foreground py-8">
            {view === "archived" ? "В архиве нет локаций" : "Локации не найдены"}
          </p>
        )}
      {!isPending && !error && data && data.totalCount > 0 && (
        <LocationsList
          {...data}
          view={view}
          onEdit={(location) => onEdit(location)}
          onDelete={(location) => onDelete(location)}
          onRestore={setSelectedRestoreLocation}
        />
      )}
      {!isPending && !error && data && (
        <Pagination
          page={data?.page || 1}
          totalPages={data?.totalPage || 1}
          onPageChange={(p) => setQuery((q) => ({ ...q, Page: p }))}
        />
      )}
      {selectedDeleteLocation && (
        <DeleteLocationDialog
          location={selectedDeleteLocation}
          onDeleted={() => handleItemLeftPage()}
          deletingClose={() => setSelectedDeleteLocation(null)}
        />
      )}

      {selectedRestoreLocation && (
        <RestoreLocationDialog
          location={selectedRestoreLocation}
          onRestored={() => handleItemLeftPage()}
          restoringClose={() => setSelectedRestoreLocation(null)}
        />
      )}

      {selectedUpdateLocation && (
        <UpdateLocationDialog
          location={selectedUpdateLocation}
          editingClose={() => setSelectedLocation(null)}
        />
      )}
      <CreateLocationDialog open={open} onClose={() => setOpen(false)} />
    </div>
  );
}
