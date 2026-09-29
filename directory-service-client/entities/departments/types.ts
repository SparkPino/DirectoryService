import { Pagination, SortDirection } from "@/shared/api/type";

export type Department = {
  departmentId: string;
  path: string;
  name: string;
  isActive: boolean;
  identifier: string;
  createdAt: string;
  updatedAt?: string;
  deletedAt?: string;
  totalCount: number;
};

export type GetDepartmentsQuery = {
  Search?: string;
  SortBy?: string;
  SortDir?: SortDirection;
  Pagination?: Pagination;
  IsActive?: boolean;
  ParentId?: string;
  LocationIds?: string[];
  ExcludeIds?: string[];
};
