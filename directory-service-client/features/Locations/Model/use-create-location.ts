import { locationApi, locationQueryOptions } from "@/entities/locations/api";
import { CreateLocationDto } from "@/entities/locations/types";
import { handleServerError } from "@/shared/api/handle-server-error";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { UseFormSetError } from "react-hook-form";
import { toast } from "sonner";
import { locationFieldMap } from "./location-field-map";

export function useCreateLocation(setError: UseFormSetError<any>) {
  const queryClient = useQueryClient();

  const { mutateAsync, isPending, error, isError } = useMutation({
    mutationFn: (location: CreateLocationDto) => locationApi.createLocation(location),
    onSettled: () => queryClient.invalidateQueries({ queryKey: locationQueryOptions.baseKey }),
    onSuccess: () => { toast.success("Локация успешно создана"); },
    onError: (error) => { handleServerError(error, setError, locationFieldMap); },
  });

  return {
    createLocation: mutateAsync,
    isError,
    isPending,
    error,
  };
}
