import { describe, expect, it } from 'vitest'
import { loginSchema, registerSchema } from './auth-schemas'

describe('auth form schemas', () => {
  it('geçerli login bilgilerini kabul eder ve kısa şifreyi reddeder', () => {
    expect(loginSchema.safeParse({ userName: 'ayse', password: 'secret1' }).success).toBe(true)
    expect(loginSchema.safeParse({ userName: 'ayse', password: '123' }).success).toBe(false)
  })

  it('kayıtta ad, e-posta ve şifre kurallarını uygular', () => {
    const valid = { firstName: 'Ayşe', lastName: 'Yılmaz', email: 'ayse@example.com', userName: 'ayse', password: 'secret1' }
    expect(registerSchema.safeParse(valid).success).toBe(true)
    expect(registerSchema.safeParse({ ...valid, firstName: 'A' }).success).toBe(false)
    expect(registerSchema.safeParse({ ...valid, email: 'hatalı' }).success).toBe(false)
  })
})
