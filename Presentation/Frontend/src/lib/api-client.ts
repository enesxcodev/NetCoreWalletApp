import { ApiError, type ApiResult } from '../types/api'
import { authToken } from './auth-token'

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5001').replace(/\/$/, '')

/** API isteklerini ortak base URL, JSON ve Bearer ayarlarıyla gönderir. */
export async function apiRequest<T>(path: string, options: { method?: 'GET' | 'POST'; body?: unknown; headers?: Record<string, string>; signal?: AbortSignal } = {}): Promise<T> {
  const token = authToken.get()
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: options.method ?? 'GET',
    headers: {
      ...(options.body === undefined ? {} : { 'Content-Type': 'application/json' }),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    },
    body: options.body === undefined ? undefined : JSON.stringify(options.body),
    signal: options.signal,
  })

  const result = response.status === 204 ? null : (await response.json().catch(() => null)) as ApiResult<T> | null
  if (response.status === 401) {
    authToken.clear()
    window.dispatchEvent(new Event('wallet:unauthorized'))
  }

  if (!response.ok || (response.status !== 204 && (!result || result.isSuccess !== true))) {
    const message = result?.errors?.filter(Boolean).join(' ') || result?.error || 'İşlem tamamlanamadı. Lütfen tekrar deneyin.'
    throw new ApiError(message, response.status)
  }

  return result?.data as T
}
