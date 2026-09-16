import { UseFormSetError } from "react-hook-form";
import { toast } from "sonner";
import { ApiRequestError } from "./ApiRequestError";

export function handleServerError(
  error: ApiRequestError,
  setError: UseFormSetError<any>,
  fieldMap: Record<string, string> = {}
) {
  const invalidField = error.firstError?.invalidField;

  if (invalidField) {
    const fieldName = fieldMap[invalidField] ?? invalidField;
    setError(fieldName, { message: error.firstError!.message });
  } else {
    toast.error(error.message || "Ошибка при создании");
  }
}
