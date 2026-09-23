import { UseFormSetError } from "react-hook-form";
import { toast } from "sonner";
import { ApiRequestError } from "./ApiRequestError";

export function handleServerError(
  error: ApiRequestError,
  setError: UseFormSetError<any>,
  fieldMap: Record<string, string> = {},
) {
  const generalMessages: string[] = [];

  for (const apiError of error.allErrors) {
    const fieldName = apiError.invalidField
      ? (fieldMap[apiError.invalidField] ?? apiError.invalidField)
      : null;

    if (fieldName) {
      setError(fieldName, { message: apiError.message });
    } else {
      generalMessages.push(apiError.message);
    }
  }

  if (generalMessages.length > 0) {
    toast.error(generalMessages.join("\n"));
  }
}
