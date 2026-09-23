import { locationApi, locationQueryOptions } from "@/entities/locations/api";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

export function useDeleteLocation() {
  const queryClient = useQueryClient();

  const { mutateAsync, isPending, error, isError } = useMutation({
    mutationFn: (id: string) => locationApi.deleteLocation(id),
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: locationQueryOptions.baseKey,
      }),
    onSuccess: () => {
      toast.success("Локация успешно удалена");
    },
    onError: (error) => toast.error(error.message),
  });

  return {
    mutateAsync,
    isPending,
    error,
    isError,
  };
}
