import { Button } from "@/shared/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/ui/dialog";
import { Field, FieldGroup } from "@/shared/ui/field";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Department } from "@/entities/departments/types";
import { handleServerError } from "@/shared/api/handle-server-error";
import {
  departmentSchema,
  DepartmentFormData,
} from "./model/department-schema";
import { departmentFieldMap } from "./model/department-field-map";
import { useUpdateDepartment } from "./model/use-update-department";

export function UpdateDepartmentDialog({
  department,
  editingClose,
}: {
  department: Department;
  editingClose: () => void;
}) {
  const {
    register,
    handleSubmit,
    setError,
    reset,
    formState: { errors, isValid },
  } = useForm<DepartmentFormData>({
    defaultValues: {
      name: department.name,
      identifier: department.identifier,
    },
    mode: "onChange",
    resolver: zodResolver(departmentSchema),
  });

  const { mutateAsync, isPending } = useUpdateDepartment();

  const onSubmit = async (data: DepartmentFormData) => {
    try {
      await mutateAsync(
        {
          id: department.departmentId,
          body: { ...data, rowVersion: department.rowVersion },
        },
        {
          onError: (error) => {
            reset(data);
            handleServerError(error, setError, departmentFieldMap);
          },
          onSuccess: () => editingClose(),
        },
      );
    } catch {}
  };

  return (
    <Dialog open onOpenChange={editingClose}>
      <DialogContent>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogHeader className="mb-4">
            <DialogTitle className="font-bold">
              Обновить подразделение
            </DialogTitle>
            <DialogDescription>
              Введите новые данные подразделения и нажмите (Обновить)
            </DialogDescription>
          </DialogHeader>

          <FieldGroup>
            <Field>
              <Label htmlFor="name">Название</Label>
              <Input id="name" {...register("name")} />
              {errors.name?.message && (
                <p className="text-destructive text-sm mt-1">
                  {errors.name.message}
                </p>
              )}
            </Field>
            <Field>
              <Label htmlFor="identifier">Идентификатор</Label>
              <Input id="identifier" {...register("identifier")} />
              {errors.identifier?.message && (
                <p className="text-destructive text-sm mt-1">
                  {errors.identifier.message}
                </p>
              )}
            </Field>
          </FieldGroup>

          <DialogFooter className="mt-6">
            <Button type="submit" disabled={isPending || !isValid}>
              {isPending ? "Обновление..." : "Обновить"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
