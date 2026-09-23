import { Button } from "@/shared/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/shared/ui/dialog";

import { Location } from "@/entities/locations/types";
import { useDeleteLocation } from "./Model/use-delete-location";
import { toast } from "sonner";
export function DeleteLocationDialog({
  location,
  deletingClose,
  onDeleted,
}: {
  location: Location;
  deletingClose: () => void;
  onDeleted?: () => void;
}) {
  const { mutateAsync, isPending } = useDeleteLocation();

  const handleConfirm = async () => {
    try {
      await mutateAsync(location.id, {
        onSuccess: () => {
          onDeleted?.();
          deletingClose();
        },
      });
    } catch {}
  };

  return (
    <Dialog open onOpenChange={deletingClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Удаление локации</DialogTitle>
          <DialogDescription>
            Вы действительно желаете удалить локацию ?
          </DialogDescription>
        </DialogHeader>
        <DialogFooter className="mt-6">
          <Button
            variant="destructive"
            disabled={isPending}
            onClick={handleConfirm}
          >
            {isPending ? "Удаление..." : "Да"}
          </Button>
          <Button onClick={deletingClose} disabled={isPending}>
            {isPending ? "..." : "Нет"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
