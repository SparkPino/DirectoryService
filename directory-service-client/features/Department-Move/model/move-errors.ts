import { ApiRequestError } from "@/shared/api/ApiRequestError";

export const MOVE_ERRORS: Record<string, string> = {
  "department.move.cycle":
    "Нельзя перенести подразделение внутрь его же дочернего",
  "department.move.parent_deleted": "Выбранное подразделение удалено",
  "department.move.parent_is_self":
    "Нельзя перенести подразделение в само себя",
  "department.move.conflict":
    "Кто-то изменяет дерево одновременно с вами, попробуйте ещё раз",
};

// Ошибки, которые относятся к выбранному родителю - показываются под select
const FIELD_ERROR_CODES = new Set([
  "department.move.cycle",
  "department.move.parent_deleted",
  "department.move.parent_is_self",
]);

export type MoveError = {
  text: string;
  isFieldError: boolean;
};

export function getMoveError(error: ApiRequestError): MoveError {
  const code = error.firstError?.code;

  const text = code && MOVE_ERRORS[code] ? MOVE_ERRORS[code] : error.message;

  const isFieldError =
    (!!code && FIELD_ERROR_CODES.has(code)) ||
    error.firstError?.type === "VALIDATION";

  return { text, isFieldError };
}