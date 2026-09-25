import { useQuery } from '@tanstack/react-query'
import { getWallet } from './api'

export const walletQueryKey = ['wallet'] as const

/** Güncel cüzdan bilgisini sorgular ve tekrar kullanıma açar. */
export function useWallet() {
  return useQuery({ queryKey: walletQueryKey, queryFn: ({ signal }) => getWallet(signal) })
}
