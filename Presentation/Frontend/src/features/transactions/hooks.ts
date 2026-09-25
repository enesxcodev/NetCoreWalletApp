import { useQuery } from '@tanstack/react-query'
import { getTransactions } from './api'

export const transactionQueryKey = ['transactions'] as const

/** İşlem geçmişini sayfa başına cache'ler. */
export function useTransactions(page: number, pageSize = 10) {
  return useQuery({ queryKey: [...transactionQueryKey, page, pageSize], queryFn: ({ signal }) => getTransactions(page, pageSize, signal) })
}
