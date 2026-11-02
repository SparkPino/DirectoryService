import { Checkbox } from "@/shared/ui/checkbox";
import { MAX_BULK_DEPARTMENTS } from "./model/bulk-limit";

type SelectAllDepartmentsProps = {
  loadedCount: number;
  isAllSelected: boolean;
  isSomeSelected: boolean;
  onToggle: () => void;
};

export function SelectAllDepartments({
  loadedCount,
  isAllSelected,
  isSomeSelected,
  onToggle,
}: SelectAllDepartmentsProps) {
  const label =
    loadedCount > MAX_BULK_DEPARTMENTS
      ? `Выбрать первые ${MAX_BULK_DEPARTMENTS} из ${loadedCount} загруженных`
      : `Выбрать все загруженные (${loadedCount})`;

  return (
    <label className="mb-3 flex w-fit cursor-pointer items-center gap-2 text-sm">
      <Checkbox
        checked={isAllSelected}
        indeterminate={isSomeSelected}
        onCheckedChange={() => onToggle()}
      />
      {label}
    </label>
  );
}
