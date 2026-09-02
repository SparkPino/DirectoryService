"use client";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { useState } from "react";
import { ApiRequestError } from "./ApiRequestError";

export function QueryProvider({ children }: { children: React.ReactNode }) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            staleTime: 5 * 60 * 1000,
            refetchOnWindowFocus: false, // При переходе на вкладку браузера, данные не будут автоматически обновляться, чтобы избежать лишних запросов к серверу.
            retry: (failureCount, error) => {
              if (
                error instanceof ApiRequestError &&
                error.status &&
                error.status < 500
              ) {
                return false; // Не повторять запросы при ошибке 404
              }
              return failureCount < 3; // Повторять запросы до 3 раз для других ошибок
            },
          },
        },
      }),
  );
  return (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
}
