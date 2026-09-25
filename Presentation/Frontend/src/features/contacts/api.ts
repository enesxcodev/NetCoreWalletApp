import { z } from 'zod'
import { apiRequest } from '../../lib/api-client'

const contactSchema = z.object({
  firstName: z.string(),
  lastName: z.string(),
  userName: z.string(),
  walletCode: z.string(),
})

const contactsPageSchema = z.object({
  items: z.array(contactSchema),
  totalCount: z.number().int().nonnegative(),
  page: z.number().int().positive(),
  pageSize: z.number().int().positive(),
})

export type TransferContact = z.infer<typeof contactSchema>
export type TransferContactsPage = z.infer<typeof contactsPageSchema>

/** Alıcı listesini ve arama sonuçlarını API sınırında doğrular. */
export async function getTransferContacts(search: string, page: number, signal?: AbortSignal): Promise<TransferContactsPage> {
  const params = new URLSearchParams({ page: String(page), pageSize: '10' })
  if (search) params.set('search', search)
  return contactsPageSchema.parse(await apiRequest<unknown>(`/api/Wallet/contacts?${params}`, { signal }))
}
