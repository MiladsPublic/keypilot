import { config } from "@/lib/config";

type ApiRequestInit = RequestInit & {
  token?: string | null;
};

export class ApiError extends Error {
  constructor(
    message: string,
    public readonly status: number
  ) {
    super(message);
  }
}

async function request<T>(path: string, init?: ApiRequestInit): Promise<T> {
  const token = init?.token;

  const response = await fetch(`${config.apiBaseUrl}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(init?.headers ?? {})
    },
    cache: "no-store"
  });

  if (!response.ok) {
    throw new ApiError(response.statusText || "Request failed", response.status);
  }

  return (await response.json()) as T;
}

export const apiClient = {
  get: <T>(path: string, token?: string | null) => request<T>(path, { token }),
  post: <TResponse, TBody>(path: string, body: TBody, token?: string | null) =>
    request<TResponse>(path, {
      method: "POST",
      body: JSON.stringify(body),
      token
    }),
  patch: <TResponse, TBody = undefined>(path: string, body?: TBody, token?: string | null) =>
    request<TResponse>(path, {
      method: "PATCH",
      body: body === undefined ? undefined : JSON.stringify(body),
      token
    }),
  delete: async (path: string, token?: string | null) => {
    const response = await fetch(`${config.apiBaseUrl}${path}`, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        ...(token ? { Authorization: `Bearer ${token}` } : {})
      },
      cache: "no-store"
    });

    if (!response.ok) {
      throw new ApiError(response.statusText || "Request failed", response.status);
    }
  }
};
