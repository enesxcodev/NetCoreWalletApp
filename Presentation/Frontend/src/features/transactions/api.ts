import { z } from 'zod'
import { apiRequest } from '../../lib/api-client'

const transactionSchema = z.object({
  walletCode: z.string(), amount: z.number(), description: z.string(),
  type: z.enum(['In', 'Out']), createdAt: z.string(),
})
const historySchema = z.array(transactionSchema)
export type TransactionItem = z.infer<typeof transactionSchema>

/** MongoDB okuma modelinden gelen işlem sayfasını doğrular. */
export async function getTransactions(page: number, pageSize: number, signal?: AbortSignal) {
  const path = `/api/Wallet/transaction?PageNumber=${page}&PageSize=${pageSize}`
  return historySchema.parse(await apiRequest<unknown>(path, { signal }))
}
