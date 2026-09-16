import { ApiError } from "./type";

export class ApiRequestError extends Error {
  private _errors: ApiError[];
  readonly status?: number;

  constructor(errors: ApiError[], status?: number) {
    super(errors[0]?.message || "Произошла ошибка");
    this.name = "ApiRequestError";
    this._errors = errors;
    this.status = status;
  }

  get firstError(): ApiError | undefined {
    return this._errors[0];
  }

  get allErrors(): ApiError[] {
    return this._errors;
  }

  get type(): string | undefined {
    return this._errors[0]?.type;
  }

  get allErrorsLikeString() : string[] {
    return this._errors.map(a=>a.message)
  }
}
