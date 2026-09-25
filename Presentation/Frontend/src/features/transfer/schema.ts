import { z } from 'zod'
import { positiveAmount } from '../../lib/amount'

export const transferSchema = z.object({
  walletCode: z.string().trim().toUpperCase().regex(/^WLT-[A-Z0-9]{8}$/, 'WLT-XXXXXXXX biçiminde bir cüzdan kodu girin.'),
  amount: positiveAmount,
  description: z.string().trim().max(200, 'Açıklama en fazla 200 karakter olabilir.'),
})

export type TransferValues = z.infer<typeof transferSchema>
