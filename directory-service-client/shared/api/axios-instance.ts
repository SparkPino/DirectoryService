import axios, { AxiosResponse } from "axios";
import { ApiRequestError } from "./ApiRequestError";
import { Envelope } from "./type";
import { dotParamsSerializer } from "./params-serializer";

export const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
  headers: { "Content-Type": "application/json" },
  paramsSerializer: { serialize: dotParamsSerializer },
});

apiClient.interceptors.response.use(
  (response: AxiosResponse<Envelope<unknown>>) => {
    const envelope = response.data;
    if (envelope.isError) {
      throw new ApiRequestError(envelope.errorList ?? [], response.status);
    }
    return response;
  },
  (error) => {
    if (axios.isAxiosError(error)) {
      if (error.response?.data.errorList) {
        throw new ApiRequestError(
          error.response.data.errorList,
          error.response.status,
        );
      }
      throw new ApiRequestError([
        { message: "Проблемы с сервером", status: 0 },
      ]);
    }
    throw error;
  },
);
