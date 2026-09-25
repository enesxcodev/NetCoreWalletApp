import { ApiError } from '../types/api'

/** API ve bağlantı hatalarını kullanıcıya güvenli Türkçe mesajlarla gösterir. */
export function errorMessage(error: unknown): string {
  if (error instanceof ApiError) return error.message
  return 'Sunucuya ulaşılamadı. Bağlantınızı kontrol edip tekrar deneyin.'
}
