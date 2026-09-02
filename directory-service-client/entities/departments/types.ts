import { Pagination, SortDirection } from "@/shared/api/type";

export type Department = {
  departmentId: number;
  path: string;
  name: string;
  createdAt: string;
  totalCount: number;
};

export type GetDepartmentsQuery = {
  Search?: string;
  SortBy?: string;
  SortDirection?: SortDirection;
  Pagination?: Pagination;
};
