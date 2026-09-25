import { z } from 'zod'

export const loginSchema = z.object({
  userName: z.string().trim().min(1, 'Kullanıcı adı zorunludur.'),
  password: z.string().min(6, 'Şifre en az 6 karakter olmalıdır.'),
})

export const registerSchema = z.object({
  firstName: z.string().trim().min(2, 'Ad en az 2 karakter olmalıdır.'),
  lastName: z.string().trim().min(1, 'Soyad zorunludur.'),
  email: z.email('Geçerli bir e-posta adresi girin.'),
  userName: z.string().trim().min(1, 'Kullanıcı adı zorunludur.'),
  password: z.string().min(6, 'Şifre en az 6 karakter olmalıdır.'),
})
