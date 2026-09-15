"use client";

import { Field } from "@/shared/ui/field";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { UseFormRegister } from "react-hook-form";

type AddressErrors = {
  country?: { message?: string };
  city?: { message?: string };
  street?: { message?: string };
  buildingNumber?: { message?: string };
  apartment?: { message?: string };
  postalCode?: { message?: string };
};

type AddressFieldsProps = {
  register: UseFormRegister<any>;
  errors?: AddressErrors;
};

export function AddressFields({ register, errors }: AddressFieldsProps) {
  return (
    <>
      <Field>
        <Label>Страна</Label>
        <Input {...register("address.country")} placeholder="Введите страну" />
        {errors?.country?.message && <p className="text-destructive text-sm mt-1">{errors.country.message}</p>}
      </Field>
      <Field>
        <Label>Город</Label>
        <Input {...register("address.city")} placeholder="Введите город" />
        {errors?.city?.message && <p className="text-destructive text-sm mt-1">{errors.city.message}</p>}
      </Field>
      <div className="flex gap-3">
        <Field className="flex-1">
          <Label>Улица</Label>
          <Input {...register("address.street")} placeholder="Введите улицу" />
          {errors?.street?.message && <p className="text-destructive text-sm mt-1">{errors.street.message}</p>}
        </Field>
        <Field className="w-32">
          <Label>Номер здания</Label>
          <Input {...register("address.buildingNumber")} placeholder="№" />
          {errors?.buildingNumber?.message && <p className="text-destructive text-sm mt-1">{errors.buildingNumber.message}</p>}
        </Field>
      </div>
      <div className="flex gap-3">
        <Field className="w-32">
          <Label>Квартира / офис</Label>
          <Input {...register("address.apartment", { setValueAs: (v) => v === "" ? null : v })} placeholder="Необязательно" />
          {errors?.apartment?.message && <p className="text-destructive text-sm mt-1">{errors.apartment.message}</p>}
        </Field>
        <Field className="flex-1">
          <Label>Почтовый индекс</Label>
          <Input {...register("address.postalCode")} placeholder="Введите индекс" />
          {errors?.postalCode?.message && <p className="text-destructive text-sm mt-1">{errors.postalCode.message}</p>}
        </Field>
      </div>
    </>
  );
}
