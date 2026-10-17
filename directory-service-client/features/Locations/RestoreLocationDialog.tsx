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
import { ArchiveRestore } from "lucide-react";
import { useRestoreLocation } from "./Model/use-restore-location";

export function RestoreLocationDialog({
  location,
  restoringClose,
  onRestored,
}: {
  location: Location;
  restoringClose: () => void;
  onRestored?: () => void;
}) {
  const { mutateAsync, isPending } = useRestoreLocation();

  const handleConfirm = async () => {
    try {
      await mutateAsync(location.id, {
        onSuccess: () => {
          onRestored?.();
          restoringClose();
        },
      });
    } catch {}
  };

  return (
    <Dialog open onOpenChange={restoringClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Восстановление локации</DialogTitle>
          <DialogDescription>
            Локация «{location.name}» вернётся в список активных.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter className="mt-6">
          <Button
            variant="outline"
            onClick={restoringClose}
            disabled={isPending}
          >
            Отмена
          </Button>
          <Button onClick={handleConfirm} disabled={isPending}>
            <ArchiveRestore />
            {isPending ? "Восстановление..." : "Восстановить"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
