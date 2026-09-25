import { z } from 'zod'

/** Türkçe virgüllü tutarı API'nin beklediği sayıya çevirir. */
export const positiveAmount = z.preprocess(
  (value) => typeof value === 'string' ? Number(value.trim().replace(',', '.')) : value,
  z.number().finite().positive('Tutar sıfırdan büyük olmalıdır.'),
)
