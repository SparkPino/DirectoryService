import { SortDirection } from "@/shared/api/type";

export type LocationQuery = {
  Search?: string;
  MinDepartmentCount?: number;
  OrderBy?: string;
  SortDirection?: SortDirection;
  Page?: number;
  PageSize?: number;
};

export type LocationAddress = {
  country: string | null;
  city: string | null;
  street: string | null;
  postalCode: string | null;
  buildingNumber: string | null;
  apartment?: string | null;
};

export type Location = {
  id: string;
  name: string;
  address: LocationAddress;
  createdAt: string;
  attachDepartmentCount: number;
  timeZone: string;
};

export type LocationDto = {
  name: string;
  address: LocationAddress;
  timezone: string;
};
export type LocationUpdate = {
  query: LocationDto;
  id: string;
};

export type UpdateLocationRequest = {
  locationName?: string;
  adressDto?: LocationAddress;
  timeZone?: string;
};
