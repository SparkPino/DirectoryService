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
  rowVersion: number;
  totalCount: number;
};

export type UpdateDepartmentRequest = {
  name?: string;
  identifier?: string;
  rowVersion: number;
};

export type DepartmentUpdate = {
  id: string;
  body: UpdateDepartmentRequest;
};
export type MoveDepartmentToNewParent = {
  departmentId: string;
  parentId: string | null;
};
export type DepartmentParentDto = {
  id: string;
  parentId: string | null;
  path: string;
  depth: number;
  updatedAt: string;
};

export type GetDepartmentsQuery = {
  Search?: string;
  SortBy?: string;
  SortDir?: SortDirection;
  Pagination?: Pagination;
  IsActive?: boolean;
  IsDeleted?: boolean;
  ParentId?: string;
  LocationIds?: string[];
  ExcludeIds?: string[];
  ExcludeSubtreeOf?: string;
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

export type MovingDepartment = {
  department: DepartmentTreeNodesDto;
  parent?: DepartmentTreeNodesDto;
};

export type GetDepartmentTreeParams = { Limit?: number; Offset?: number };
