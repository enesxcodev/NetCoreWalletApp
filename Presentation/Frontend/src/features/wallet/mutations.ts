import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiRequest } from '../../lib/api-client'
import { transactionQueryKey } from '../transactions/hooks'
import { walletQueryKey } from './hooks'

export type MoneyAction = 'deposit' | 'withdraw'

/** Para yatırma veya çekme sonrası ilgili cache'leri yeniler. */
export function useMoneyMutation(action: MoneyAction) {
  const client = useQueryClient()
  return useMutation({
    mutationFn: (amount: number) => apiRequest<void>(`/api/Wallet/${action}`, { method: 'POST', body: { amount } }),
    onSuccess: async () => {
      await Promise.all([client.invalidateQueries({ queryKey: walletQueryKey }), client.invalidateQueries({ queryKey: transactionQueryKey })])
    },
  })
}
