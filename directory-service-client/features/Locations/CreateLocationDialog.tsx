"use client";

import { Button } from "@/shared/ui/button";
import { DialogFooter, DialogContent, Dialog, DialogHeader, DialogDescription, DialogTitle } from "@/shared/ui/dialog";
import { Field, FieldGroup } from "@/shared/ui/field";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { useCreateLocation } from "./Model/use-create-location";
import { AddressFields } from "@/entities/locations/ui/AddressFields";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

const createLocationSchema = z.object({
  name: z.string()
  .min(3 , "Название должно быть не менее 3 символов")
  .max(120, "Название должно быть не более 120 символов"),
  timezone: z.string()
  .min(2, "Временная зона должна быть не менее 2 символов")
  .max(20, "Временная зона должна быть не более 20 символов")
  .regex(/^[a-zA-Z]+\/[a-zA-Z_]+$/, "Временная зона должна быть в формате 'Continent/City'"),
  address: 
  z.object({
      country: z.string()
       .min(3 , "Название должно быть не менее 3 символов")
      .nullable(),
      city: z.string()
       .min(3 , "Название должно быть не менее 3 символов")
      .nullable(),
    street: z.string()
     .min(3 , "Название должно быть не менее 3 символов")
    .nullable(),
    postalCode: z.string()
     .min(3 , "Название должно быть не менее 3 символов")
    .nullable(),
    buildingNumber: z.string()
     .min(3 , "Название должно быть не менее 3 символов")
    .nullable(),
    apartment: z.string()
     .min(3 , "Название должно быть не менее 3 символов")
    .nullable(),
  }),
});

type CreateLocationFormData = z.infer<typeof createLocationSchema>;

const initialForm: CreateLocationFormData = {
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

export function CreateLocationDialog({ open, onClose }: { open: boolean; onClose: () => void }) {

  const { register, handleSubmit, reset, formState: { errors, isValid } } = useForm<CreateLocationFormData>({
    defaultValues: initialForm,
    resolver: zodResolver(createLocationSchema),
    mode: "onChange", // Валидация будет происходить при изменении полей
  });

  const { createLocation, isPending } = useCreateLocation();

  const onSubmit = async (data: CreateLocationFormData) => {
    try {
      await createLocation(data, {
        onSuccess: () => {
          reset(initialForm);
          onClose();
        }
      });
    } catch {
      // помилка вже обробляється в хуку через toast
    }
  };

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogHeader>
            <DialogTitle>Создать локацию</DialogTitle>
            <DialogDescription>
              Введите название локации и нажмите (Сохранить)
            </DialogDescription>
          </DialogHeader>

          <FieldGroup>
            <Field>
              <Label htmlFor="name">Название</Label>
              <Input
                id="name"
                placeholder="Введите название локации"
                {...register("name")}
              />
              {errors.name && 
              <p className="text-destructive text-sm mt-1">{errors.name.message}</p>}
              
            </Field>
            <Field>
              <Label htmlFor="timezone">Регион (timezone)</Label>
              <Input
                id="timezone"
                placeholder="Europe/Warsaw"
                {...register("timezone")}
              />
              {errors.timezone && 
              <p className="text-destructive text-sm mt-1">{errors.timezone.message}</p>}
            </Field>   
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