export type LocationQuery = {
  Search?: string;
  MinDepartmentCount?: number;
  OrderBy?: string;
  SortDirection?: SortDirection;
  Page?: number;
  PageSize?: number;
};

export type SortDirection = "ASC" | "DESC";

export type LocationAddress = {
  country: string | null;
  city: string | null;
  street: string | null;
  postalCode: string | null;
  buildingNumber: string | null;
  apartment: string | null;
};

export type Location = {
  id: string;
  name: string;
  address: LocationAddress;
  createdAt: string;
  attachDepartmentCount: number;
};

export type PagedResult<T> = {
  items: T[];
  totalCount: number;
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
