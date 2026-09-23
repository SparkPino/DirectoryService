"use client";

import { Field } from "@/shared/ui/field";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { UseFormRegister } from "react-hook-form";

type LocationNameErrors = {
  name?: { message?: string };
  timezone?: { message?: string };
};

type LocationNameFieldsProps = {
  register: UseFormRegister<any>;
  errors?: LocationNameErrors;
};

export function LocationNameFields({ register, errors }: LocationNameFieldsProps) {
  return (
    <>
      <Field>
        <Label htmlFor="name">Название</Label>
        <Input
          id="name"
          placeholder="Введите название локации"
          {...register("name")}
        />
        {errors?.name?.message && (
          <p className="text-destructive text-sm mt-1">{errors.name.message}</p>
        )}
      </Field>
      <Field>
        <Label htmlFor="timezone">Регион (timezone)</Label>
        <Input
          id="timezone"
          placeholder="Europe/Warsaw"
          {...register("timezone")}
        />
        {errors?.timezone?.message && (
          <p className="text-destructive text-sm mt-1">{errors.timezone.message}</p>
        )}
      </Field>
    </>
  );
}
