import * as z from "zod";

export const locationSchema = z.object({
  name: z
    .string()
    .min(2, "Название должно быть не менее 2 символов")
    .max(120, "Название должно быть не более 120 символов"),
  timezone: z
    .string()
    .min(2, "Временная зона должна быть не менее 2 символов")
    .max(20, "Временная зона должна быть не более 20 символов")
    .regex(
      /^[A-Za-z]+(?:\/[A-Za-z_\-]+)+$/,
      "Временная зона должна быть в формате 'Continent/City'",
    ),
  address: z.object({
    country: z
      .string()
      .min(3, "Название должно быть не менее 3 символов")
      .nullable(),
    city: z
      .string()
      .min(3, "Название должно быть не менее 3 символов")
      .nullable(),
    street: z
      .string()
      .min(1, "Поле обязательно для заполнения")
      .min(3, "Название должно быть не менее 3 символов")
      .nullable(),
    postalCode: z
      .string()
      .min(3, "Название должно быть не менее 3 символов")
      .nullable(),
    buildingNumber: z
      .string()
      .min(1, "Поле обязательно для заполнения")
      .nullable(),
    apartment: z
      .string()
      .max(10, "Квартира/офис должна быть не более 10 символов")
      .nullable(),
  }),
});

export type LocationFormData = z.infer<typeof locationSchema>;