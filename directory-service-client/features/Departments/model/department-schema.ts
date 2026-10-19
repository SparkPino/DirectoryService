import * as z from "zod";

// правила совпадают с DepartmentName / DepartmentIdentifier на бэкенде
export const departmentSchema = z.object({
  name: z
    .string()
    .trim()
    .min(3, "Название должно быть не менее 3 символов")
    .max(150, "Название должно быть не более 150 символов"),
  identifier: z
    .string()
    .trim()
    .min(4, "Идентификатор должен быть не менее 4 символов")
    .max(150, "Идентификатор должен быть не более 150 символов")
    .regex(/^[A-Za-z]+$/, "Идентификатор может содержать только латинские буквы"),
});

export type DepartmentFormData = z.infer<typeof departmentSchema>;
