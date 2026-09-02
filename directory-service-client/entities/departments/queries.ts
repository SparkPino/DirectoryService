import { queryOptions } from "@tanstack/react-query";
import { GetDepartmentsQuery } from "./types";
import { departmentApi } from "./api";

export const departmentQueries = {
  list: (query: GetDepartmentsQuery) =>
    queryOptions({
      queryKey: ["departments", query],
      queryFn: ({ signal }) => departmentApi.getDepartments({ query, signal }),
      retry: false,
    }),
};
