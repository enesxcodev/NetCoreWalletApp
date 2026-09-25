import { z } from 'zod'
import { apiRequest } from '../../lib/api-client'

const walletSchema = z.object({ code: z.string(), balance: z.number() })
export type WalletSummary = z.infer<typeof walletSchema>

/** Cüzdan özetini API sınırında doğrulayarak döndürür. */
export async function getWallet(signal?: AbortSignal): Promise<WalletSummary> {
  return walletSchema.parse(await apiRequest<unknown>('/api/Wallet', { signal }))
}
