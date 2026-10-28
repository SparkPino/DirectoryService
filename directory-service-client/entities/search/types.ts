export type SearchQuery = {
  Search: string;
  Limit?: number;
};

export type SearchResultItem = {
  type: SearchResultType;
  id: string;
  title: string;
  subtitle: string | null;
};

export type SearchResultType = "Department" | "Location" | "Position";
