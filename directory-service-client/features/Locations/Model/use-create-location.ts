import { locationApi, locationQueryOptions } from "@/entities/locations/api";
import { CreateLocationDto } from "@/entities/locations/types";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

export function useCreateLocation() {
    const queryClient = useQueryClient();
    const { mutateAsync, isPending, error } = useMutation(
       {
        mutationFn: (location: CreateLocationDto) => locationApi.createLocation(location),
        onSettled: () => queryClient.invalidateQueries({queryKey: locationQueryOptions.baseKey}),
        onSuccess: () => {  toast.success("Локация успешно создана");},
        onError: (error) => { toast.error(error instanceof Error ? error.message : "Ошибка при создании локации");}
       }
    );


    return {
        createLocation: mutateAsync,
        isPending,
        error
    };
}