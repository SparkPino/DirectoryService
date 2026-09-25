function flatten(
  value: unknown,
  prefix: string,
  out: URLSearchParams,
): void {
  if (value === undefined || value === null) return;

  // Массивы передаём повторяющимся ключом (ids=a&ids=b) — такой формат понимает ASP.NET [FromQuery]
  if (Array.isArray(value)) {
    for (const item of value) {
      flatten(item, prefix, out);
    }
    return;
  }

  if (typeof value !== "object" || value instanceof Date) {
    out.append(prefix, String(value));
    return;
  }

  for (const [key, child] of Object.entries(value)) {
    flatten(child, prefix ? `${prefix}.${key}` : key, out);
  }
}

export function dotParamsSerializer(params: Record<string, unknown>): string {
  const out = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    flatten(value, key, out);
  }
  return out.toString();
}