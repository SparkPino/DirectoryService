import PositionPage from "@/widgets/positions/position-page";

export const metadata = { title: "Позиции" };

export default function PositionsPage() {
  return (
    <div className="max-w-2xl mx-auto py-10 px-4">
      <h1 className="text-2xl font-semibold mb-6 text-center">Позиции</h1>
      <PositionPage />
    </div>
  );
}
