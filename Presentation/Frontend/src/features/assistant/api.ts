import { apiRequest } from '../../lib/api-client'

interface AssistantMessageResponse {
  reply: string
  contacts: TransferContact[] | null
}

export interface TransferContact {
  firstName: string
  lastName: string
  userName: string
  walletCode: string
}

/** Mesajı oturum açmış kullanıcının cüzdan asistanına gönderir. */
export function sendAssistantMessage(message: string, signal?: AbortSignal) {
  return apiRequest<AssistantMessageResponse>('/api/Assistant/message', {
    method: 'POST',
    body: { message },
    signal,
  })
}
