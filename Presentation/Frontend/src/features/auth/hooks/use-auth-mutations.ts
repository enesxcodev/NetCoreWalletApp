import { useMutation } from '@tanstack/react-query'
import { apiRequest } from '../../../lib/api-client'
import type { LoginPayload, RegisterPayload } from '../types/auth'

/** Kimlik doğrulama isteklerini cache ve sayfa bileşenlerinden ayrı tutar. */
export function useLoginMutation() {
  return useMutation({
    mutationFn: (payload: LoginPayload) => apiRequest<string>('/api/Auth/login', { method: 'POST', body: payload }),
  })
}

/** Yeni hesap açma isteğini backend'in kayıt sözleşmesine göre gönderir. */
export function useRegisterMutation() {
  return useMutation({
    mutationFn: (payload: RegisterPayload) => apiRequest<string>('/api/Auth/register', { method: 'POST', body: payload }),
  })
}
