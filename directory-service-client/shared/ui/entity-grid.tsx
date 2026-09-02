import { ReactNode } from "react";

type EntityGridProps<T> = {
  items: T[];
  getKey: (item: T) => string;
  renderItem: (item: T) => ReactNode;
};

export function EntityGrid<T>({ items, getKey, renderItem }: EntityGridProps<T>) {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
      {items.map((item) => (
        <div key={getKey(item)}>{renderItem(item)}</div>
      ))}
    </div>
  );
}
