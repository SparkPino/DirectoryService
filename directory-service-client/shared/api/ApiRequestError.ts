export class ApiRequestError extends Error {
  code?: string;
  type?: string;
  invalidField?: string | null;
  constructor(
    message: string,
    code?: string,
    type?: string,
    invalidField?: string | null,
  ) {
    super(message);
    this.name = "ApiRequestError";
    this.code = code;
    this.type = type;
    this.invalidField = invalidField;
  }
}
