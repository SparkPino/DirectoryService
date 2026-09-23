import { Button } from "@/shared/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/ui/dialog";
import { FieldGroup } from "@/shared/ui/field";
import { locationSchema, LocationFormData } from "./Model/location-schema";
import useUpdateLocation from "./Model/use-update-location";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Location } from "@/entities/locations/types";
import { locationFieldMap } from "./Model/location-field-map";
import { handleServerError } from "@/shared/api/handle-server-error";
import { AddressFields } from "@/entities/locations/ui/AddressFields";
import { LocationNameFields } from "@/entities/locations/ui/LocationNameFields";

export function UpdateLocationDialog({
  location,
  editingClose,
}: {
  location: Location;
  editingClose: () => void;
}) {
  const initialForm: LocationFormData = {
    name: location.name,
    timezone: location.timeZone,
    address: {
      country: location.address.country,
      city: location.address.city,
      street: location.address.street,
      postalCode: location.address.postalCode,
      buildingNumber: location.address.buildingNumber,
      apartment: location.address.apartment || null,
    },
  };

  const {
    register,
    handleSubmit,
    setError,
    reset,
    formState: { errors, isValid },
  } = useForm<LocationFormData>({
    defaultValues: initialForm,
    mode: "onChange",
    resolver: zodResolver(locationSchema),
  });

  const { mutateAsync, isPending } = useUpdateLocation();

  const onSubmit = async (data: LocationFormData) => {
    try {
      await mutateAsync(
        { id: location.id, query: data },
        {
          onError: (error) => {
            reset(data);
            handleServerError(error, setError, locationFieldMap);
          },
          onSuccess: () => {
            editingClose();
          },
        },
      );
    } catch {}
  };

  return (
    <Dialog open onOpenChange={editingClose}>
      <DialogContent>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogHeader className="mb-4">
            <DialogTitle className={"font-bold"}>Обновить локацию</DialogTitle>
            <DialogDescription>
              Введите новые данные локации и нажмите (Сохранить)
            </DialogDescription>
          </DialogHeader>

          <FieldGroup>
            <LocationNameFields register={register} errors={errors} />
            <AddressFields register={register} errors={errors.address as any} />
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
