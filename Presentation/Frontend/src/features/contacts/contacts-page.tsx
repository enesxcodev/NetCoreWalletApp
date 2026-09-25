import { ChevronLeft, ChevronRight, UsersRound } from 'lucide-react'
import { useEffect, useState } from 'react'
import { QueryError, QuerySkeleton } from '../../components/query-state'
import { ContactSearch } from './components/contact-search'
import { ContactsTable } from './components/contacts-table'
import { useTransferContacts } from './hooks'

/** Aramayı geciktirerek API'ye gönderir; sonuç tablosu ve sayfalamayı yönetir. */
export function ContactsPage() {
  const [search, setSearch] = useState('')
  const [debouncedSearch, setDebouncedSearch] = useState('')
  const [page, setPage] = useState(1)

  useEffect(() => {
    const timeout = window.setTimeout(() => setDebouncedSearch(search.trim()), 300)
    return () => window.clearTimeout(timeout)
  }, [search])

  const contacts = useTransferContacts(debouncedSearch, page)
  const isTyping = search.trim() !== debouncedSearch
  const changeSearch = (value: string) => { setSearch(value); setPage(1) }
  const totalPages = contacts.data ? Math.max(1, Math.ceil(contacts.data.totalCount / contacts.data.pageSize)) : 1

  return <div className="content-stack">
    <header className="page-heading"><span className="card-kicker">TRANSFER REHBERİ</span><h1>Transfer Kişileri</h1><p>Alıcıyı bulun, cüzdan kodunu kopyalayın ve transferinizde kullanın.</p></header>
    <section className="content-section contacts-section"><div className="contacts-section-top"><span className="contacts-section-icon"><UsersRound size={21} /></span><div><h2>Kişiler</h2><p>Cüzdanı olan kullanıcılar listelenir.</p></div></div>
      <ContactSearch value={search} onChange={changeSearch} />
      <div className="contacts-results-heading"><strong>Sonuçlar</strong><span>{contacts.data && !isTyping ? `${contacts.data.totalCount} kişi` : 'Aranıyor...'}</span></div>
      {isTyping || contacts.isPending ? <QuerySkeleton /> : contacts.isError ? <QueryError retry={() => void contacts.refetch()} /> : <><ContactsTable contacts={contacts.data.items} searched={Boolean(debouncedSearch)} /><div className="pagination"><button type="button" onClick={() => setPage((current) => current - 1)} disabled={page === 1}><ChevronLeft size={16} /> Önceki</button><span>Sayfa {page} / {totalPages}</span><button type="button" onClick={() => setPage((current) => current + 1)} disabled={page >= totalPages}>Sonraki <ChevronRight size={16} /></button></div></>}
    </section>
  </div>
}
