import { locationApi, locationQueryOptions } from "@/entities/locations/api";
import { LocationUpdate } from "@/entities/locations/types";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

export default function useUpdateLocation() {
  const queryClient = useQueryClient();

  const { mutateAsync, isPending, error, isError } = useMutation({
    mutationFn: (query: LocationUpdate) => locationApi.updateLocation(query),
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: locationQueryOptions.baseKey,
      }),
    onSuccess: () => {
      toast.success("Локация успешно обновлена");
    },
  });

  return {
    mutateAsync,
    isPending,
    error,
    isError,
  };
}
