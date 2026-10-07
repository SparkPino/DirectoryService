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
  DepartmentIds?: string[];
};

export type DepartmentTreeNodesDto = {
  id: string;
  parentId: string | null;
  name: string;
  identifier: string;
  path: string;
  depth: number;
  hasChildren: boolean;
  childrenCount: number | null;
};

export type GetDepartmentTreeParams = { Limit?: number; Offset?: number };
