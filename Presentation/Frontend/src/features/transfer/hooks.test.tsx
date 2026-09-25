import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { act, renderHook, waitFor } from '@testing-library/react'
import { afterEach, describe, expect, it, vi } from 'vitest'
import { useTransfer } from './hooks'

describe('transfer mutation idempotency desteği', () => {
  afterEach(() => vi.unstubAllGlobals())

  it('aynı mantıksal transfer için tekrar denendiğinde aynı anahtarı yollar', async () => {
    const fetchMock = vi.fn().mockImplementation(() => Promise.resolve(new Response(JSON.stringify({ isSuccess: true, data: '08d6e623-e3cf-4a68-a72f-a982bd7775ac' }), { status: 200 })))
    vi.stubGlobal('fetch', fetchMock)
    const client = new QueryClient({ defaultOptions: { queries: { retry: false }, mutations: { retry: false } } })
    const wrapper = ({ children }: { children: React.ReactNode }) => <QueryClientProvider client={client}>{children}</QueryClientProvider>
    const { result } = renderHook(() => useTransfer(), { wrapper })
    const payload = { values: { walletCode: 'WLT-12345678', amount: 15, description: 'Test' }, idempotencyKey: 'same-logical-transfer' }
    await act(async () => { await result.current.mutateAsync(payload); await result.current.mutateAsync(payload) })
    await waitFor(() => expect(fetchMock).toHaveBeenCalledTimes(2))
    for (const [, options] of fetchMock.mock.calls) expect((options as RequestInit).headers).toMatchObject({ 'Idempotency-Key': 'same-logical-transfer' })
    client.clear()
  })
})
