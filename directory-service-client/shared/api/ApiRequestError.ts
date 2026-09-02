export class ApiRequestError extends Error {
  code?: string;
  type?: string;
  invalidField?: string | null;
  status?: number;
  constructor(
    message: string,
    code?: string,
    type?: string,
    invalidField?: string | null,
    status?: number,
  ) {
    super(message);
    this.name = "ApiRequestError";
    this.code = code;
    this.type = type;
    this.invalidField = invalidField;
    this.status = status;
  }
}
