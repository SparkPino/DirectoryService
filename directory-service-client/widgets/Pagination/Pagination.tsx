import { PaginationProps } from "@/shared/api/type";
import {
  PaginationContent,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/shared/ui/pagination";

export function getPageNumbers(
  current: number,
  total: number,
  visible: number,
) {
  const half = Math.floor(visible / 2);
  let start = Math.max(1, current - half);
  const end = Math.min(total, start + visible - 1);
  start = Math.max(1, end - visible + 1);

  const pages: number[] = [];

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }
  return pages;
}

export default function Pagination({
  page,
  totalPages,
  onPageChange,
}: PaginationProps) {
  const pageNumbers = getPageNumbers(page, totalPages, 5);
  return (
    <PaginationContent>
      <PaginationItem>
        <PaginationPrevious
          onClick={() => page > 0 && onPageChange(page - 1)}
          className={page <= 1 ? "pointer-events-none opacity-50" : undefined}
          text={"previous"}
        />
      </PaginationItem>

      {pageNumbers.map((item) => (
        <PaginationItem key={item}>
          <PaginationLink
            isActive={item === page}
            onClick={() => onPageChange(item)}
          >
            {item}
          </PaginationLink>
        </PaginationItem>
      ))}
      <PaginationItem>
        <PaginationNext
          onClick={() => page < totalPages && onPageChange(page + 1)}
          className={
            page >= totalPages ? "pointer-events-none opacity-50" : undefined
          }
          text={"next"}
        />
      </PaginationItem>
    </PaginationContent>
  );
}
