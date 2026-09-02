import { Dispatch, SetStateAction } from "react";

export type PagedResult<T> = {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPage: number;
};

export type Envelope<T> = {
  result: T | null;
  errorList: ApiError[] | null;
  isError: boolean;
  timeGenerated: string;
};
export type SortDirection = "ASC" | "DESC";

export type Pagination = {
  Page?: number;
  PageSize?: number;
};
export type ApiError = {
  code?: string;
  message: string;
  type?: string;
  invalidField?: string | null;
  status: number;
};

declare module "@tanstack/react-query" {
  interface Register {
    defaultError: ApiError;
  }
}
export type FilterProps<TQuery> = {
  query: TQuery;
  onChange: Dispatch<SetStateAction<TQuery>>;
};

export type PaginationProps = {
  page: number;
  totalPages: number;
  onPageChange: (page: number) => void;
};
