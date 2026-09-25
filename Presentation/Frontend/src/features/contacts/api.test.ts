import { afterEach, describe, expect, it, vi } from 'vitest'
import { getTransferContacts } from './api'

afterEach(() => vi.unstubAllGlobals())

describe('transfer kişileri API istemcisi', () => {
  it('arama terimini kodlayıp sayfalı sonuçları okur', async () => {
    const fetchMock = vi.fn().mockResolvedValue(new Response(JSON.stringify({
      isSuccess: true,
      data: { items: [{ firstName: 'Ayşe', lastName: 'Yılmaz', userName: 'ayse', walletCode: 'WLT-12345678' }], totalCount: 1, page: 1, pageSize: 10 },
    }), { status: 200 }))
    vi.stubGlobal('fetch', fetchMock)

    const result = await getTransferContacts('Ayşe Yılmaz', 1)

    expect(fetchMock.mock.calls[0][0]).toContain('search=Ay%C5%9Fe+Y%C4%B1lmaz')
    expect(result.items[0].walletCode).toBe('WLT-12345678')
    expect(result.totalCount).toBe(1)
  })
})
