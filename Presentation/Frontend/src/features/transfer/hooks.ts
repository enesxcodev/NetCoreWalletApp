import { useMutation, useQueryClient } from '@tanstack/react-query'
import { z } from 'zod'
import { apiRequest } from '../../lib/api-client'
import { transactionQueryKey } from '../transactions/hooks'
import { walletQueryKey } from '../wallet/hooks'
import type { TransferValues } from './schema'

interface TransferRequest { values: TransferValues; idempotencyKey: string }

/** Aynı mantıksal transferin tekrarında verilen UUID'yi koruyarak isteği gönderir. */
export function useTransfer() {
  const client = useQueryClient()
  return useMutation({
    mutationFn: async ({ values, idempotencyKey }: TransferRequest) => {
      const result = await apiRequest<unknown>('/api/Wallet/transfer', {
        method: 'POST', body: values, headers: { 'Idempotency-Key': idempotencyKey },
      })
      return z.string().uuid().parse(result)
    },
    onSuccess: async () => {
      await Promise.all([client.invalidateQueries({ queryKey: walletQueryKey }), client.invalidateQueries({ queryKey: transactionQueryKey })])
    },
  })
}
