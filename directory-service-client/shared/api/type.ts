export type PagedResult<T> = {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
};

export type ApiError = {
  code?: string;
  message: string;
  type?: string;
  invalidField?: string | null;
};

export type Envelope<T> = {
  result: T | null;
  errorList: ApiError[] | null;
  isError: boolean;
  timeGenerated: string;
};
export type SortDirection = "ASC" | "DESC";

export type Pagination = {
  Page: number;
  PageSize: number;
};
