import { LocationView } from "@/entities/locations/ui/LocationsList";
import { usePathname, useRouter, useSearchParams } from "next/navigation";

const PARAM = "view";

export function useLocationView() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const pathname = usePathname();
  const raw = searchParams.get(PARAM);
  const view: LocationView = raw === "archived" ? "archived" : "active";

  function setLocationView(locationView: LocationView) {
    const params = new URLSearchParams(searchParams);
    params.set(PARAM, locationView);
    router.replace(`${pathname}?${params.toString()}`, { scroll: false });
  }

  return {
    view,
    setLocationView,
  };
}
