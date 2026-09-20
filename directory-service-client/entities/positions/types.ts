import { Pagination, SortDirection } from "@/shared/api/type";

export type Position = {
  id: string;
  name: string;
  description: string | null;
  createdAt: string;
  attachDepartmentCount: number;
};

export type GetPositionsQuery = {
  Search?: string;
  SortBy?: string;
  SortDir?: SortDirection;
  Pagination?: Pagination;
};
