import { describe, expect, it } from 'vitest'
import { transferSchema } from './schema'

describe('transfer formu', () => {
  it('Türkçe ondalık tutarı sayıya çevirir', () => {
    const result = transferSchema.parse({ walletCode: 'WLT-12345678', amount: '123,45', description: '' })
    expect(result.amount).toBe(123.45)
  })

  it('sıfır tutarı ve boş alıcı kodunu reddeder', () => {
    expect(transferSchema.safeParse({ walletCode: '', amount: '10', description: '' }).success).toBe(false)
    expect(transferSchema.safeParse({ walletCode: 'WLT-12345678', amount: '0', description: '' }).success).toBe(false)
  })
})
