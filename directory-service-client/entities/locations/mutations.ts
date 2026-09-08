import { CreateLocationsMutation } from "./types";

export function locationMutations() {
  return {
    create: (location: CreateLocationsMutation) => {},
  };
}
