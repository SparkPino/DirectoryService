function flatten(
  value: unknown,
  prefix: string,
  out: Record<string, string>,
): void {
  if (value === undefined || value === null) return;

  if (typeof value !== "object" || value instanceof Date) {
    out[prefix] = String(value);
    return;
  }

  for (const [key, child] of Object.entries(value)) {
    flatten(child, prefix ? `${prefix}.${key}` : key, out);
  }
}

export function dotParamsSerializer(params: Record<string, unknown>): string {
  const flat: Record<string, string> = {};
  for (const [key, value] of Object.entries(params)) {
    flatten(value, key, flat);
  }
  return new URLSearchParams(flat).toString();
}
