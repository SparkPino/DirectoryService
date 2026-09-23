"use client";

import { Button } from "@/shared/ui/button";
import {
  DialogFooter,
  DialogContent,
  Dialog,
  DialogHeader,
  DialogDescription,
  DialogTitle,
} from "@/shared/ui/dialog";
import { FieldGroup } from "@/shared/ui/field";
import { useCreateLocation } from "./Model/use-create-location";
import { AddressFields } from "@/entities/locations/ui/AddressFields";
import { LocationNameFields } from "@/entities/locations/ui/LocationNameFields";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { locationSchema, LocationFormData } from "./Model/location-schema";
import { locationFieldMap } from "./Model/location-field-map";
import { handleServerError } from "@/shared/api/handle-server-error";

const initialForm: LocationFormData = {
  name: "",
  timezone: "",
  address: {
    country: null,
    city: null,
    street: null,
    postalCode: null,
    buildingNumber: null,
    apartment: null,
  },
};

export function CreateLocationDialog({
  open,
  onClose,
}: {
  open: boolean;
  onClose: () => void;
}) {
  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors, isValid },
  } = useForm<LocationFormData>({
    defaultValues: initialForm,
    resolver: zodResolver(locationSchema),
    mode: "onChange", // Валидация будет происходить при изменении полей
  });

  const { createLocation, isPending } = useCreateLocation();

  const onSubmit = async (data: LocationFormData) => {
    try {
      await createLocation(data, {
        onError: (error) => {
          handleServerError(error, setError, locationFieldMap);
        },
        onSuccess: () => {
          reset(initialForm);
          onClose();
        },
      });
    } catch {}
  };

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogHeader className="mb-4">
            <DialogTitle className={"font-bold"}>Создать локацию</DialogTitle>
            <DialogDescription>
              Введите название локации и нажмите (Сохранить)
            </DialogDescription>
          </DialogHeader>

          <FieldGroup>
            <LocationNameFields register={register} errors={errors} />
            <AddressFields register={register} errors={errors.address as any} />
          </FieldGroup>

          <DialogFooter className="mt-6">
            <Button type="submit" disabled={isPending || !isValid}>
              {isPending ? "Создание..." : "Создать"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
