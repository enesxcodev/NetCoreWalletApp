import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { apiRequest } from './api-client'
import { authToken } from './auth-token'

describe('ortak API istemcisi', () => {
  beforeEach(() => { authToken.set('test-token') })
  afterEach(() => { vi.unstubAllGlobals(); authToken.clear() })

  it('Bearer ve Idempotency-Key header değerlerini korur', async () => {
    const fetchMock = vi.fn().mockResolvedValue(new Response(JSON.stringify({ isSuccess: true, data: 'transaction-id' }), { status: 200 }))
    vi.stubGlobal('fetch', fetchMock)
    await apiRequest('/api/Wallet/transfer', { method: 'POST', body: { amount: 10 }, headers: { 'Idempotency-Key': 'same-key' } })
    const options = fetchMock.mock.calls[0][1] as RequestInit
    expect(options.headers).toMatchObject({ Authorization: 'Bearer test-token', 'Idempotency-Key': 'same-key' })
  })

  it('401 cevabında oturumu temizler', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response(JSON.stringify({ isSuccess: false, error: 'Oturum bitti' }), { status: 401 })))
    await expect(apiRequest('/api/Wallet')).rejects.toThrow('Oturum bitti')
    expect(authToken.get()).toBeNull()
  })

  it('204 başarılı para işlemini boş cevapla kabul eder', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response(null, { status: 204 })))
    await expect(apiRequest<void>('/api/Wallet/deposit', { method: 'POST', body: { amount: 10 } })).resolves.toBeUndefined()
  })
})
