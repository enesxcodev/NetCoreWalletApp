import { useQuery } from '@tanstack/react-query'
import { getTransferContacts } from './api'

/** Arama terimi ve sayfa numarasına göre alıcıları ayrı ayrı cache'ler. */
export function useTransferContacts(search: string, page: number) {
  return useQuery({
    queryKey: ['transfer-contacts', search, page],
    queryFn: ({ signal }) => getTransferContacts(search, page, signal),
  })
}
