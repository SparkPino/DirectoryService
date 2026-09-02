import axios, { AxiosResponse } from "axios";
import { ApiRequestError } from "./ApiRequestError";
import { Envelope } from "./type";

export const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
  headers: { "Content-Type": "application/json" },
});

apiClient.interceptors.response.use(
  (response: AxiosResponse<Envelope<unknown>>) => {
    const envelope = response.data;
    if (envelope.isError) {
      const firstError = envelope.errorList?.[0];
      throw new ApiRequestError(
        firstError?.message || "Произошла ошибка",
        firstError?.code,
        firstError?.type,
        firstError?.invalidField,
        firstError?.status,
      );
    }
    return response;
  },
  (error) => {
    if (axios.isAxiosError(error) && error.response?.data.errorList) {
      const firstError = error.response.data.errorList[0];
      if (firstError) {
        throw new ApiRequestError(
          firstError?.message || "Произошла ошибка",
          firstError?.code,
          firstError?.type,
          firstError?.invalidField,
          firstError?.status,
        );
      }
    }
    throw error;
  },
);
