import { Spinner } from "@/shared/ui/spinner";
import LocationsPage from "@/widgets/locations/location-page";
import { Suspense } from "react";

export const metadata = { title: "Локации" };

export default function Page() {
  return (
    <Suspense
      fallback={
        <div className="flex justify-center py-8">
          <Spinner className="size-6" />
        </div>
      }
    >
      <LocationsPage />
    </Suspense>
  );
}
