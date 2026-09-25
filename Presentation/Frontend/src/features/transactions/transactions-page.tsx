import { ChevronLeft, ChevronRight, X } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import { QueryError, QuerySkeleton } from '../../components/query-state'
import { formatDate, formatMoney } from '../../lib/format'
import { TransactionList } from './components/transaction-list'
import type { TransactionItem } from './api'
import { useTransactions } from './hooks'

const PAGE_SIZE = 10

/** İşlem geçmişini sayfalar; seçilen işlemi ayrıntı panelinde gösterir. */
export function TransactionsPage() {
  const [page, setPage] = useState(1)
  const [selected, setSelected] = useState<TransactionItem | null>(null)
  const closeButton = useRef<HTMLButtonElement>(null)
  const history = useTransactions(page, PAGE_SIZE)
  useEffect(() => {
    if (!selected) return
    closeButton.current?.focus()
    const onKeyDown = (event: KeyboardEvent) => { if (event.key === 'Escape') setSelected(null) }
    window.addEventListener('keydown', onKeyDown)
    return () => window.removeEventListener('keydown', onKeyDown)
  }, [selected])
  return <div className="content-stack"><header className="page-heading"><span className="card-kicker">HESAP HAREKETLERİ</span><h1>İşlem geçmişi</h1><p>Gelen ve giden transferlerinizi görüntüleyin.</p></header><section className="content-section"><div className="section-heading"><div><h2>Tüm transferler</h2><p>En yeni işlemler önce gösterilir.</p></div></div>
    {history.isPending ? <QuerySkeleton /> : history.isError ? <QueryError retry={() => void history.refetch()} /> : <><TransactionList items={history.data} onSelect={setSelected} /><div className="pagination"><button type="button" onClick={() => { setPage(page - 1); setSelected(null) }} disabled={page === 1}><ChevronLeft size={16} /> Önceki</button><span>Sayfa {page}</span><button type="button" onClick={() => { setPage(page + 1); setSelected(null) }} disabled={history.data.length < PAGE_SIZE}>Sonraki <ChevronRight size={16} /></button></div></>}
  </section>{selected && <div className="detail-backdrop" onClick={() => setSelected(null)}><section className="detail-panel" role="dialog" aria-modal="true" aria-labelledby="detail-title" onClick={(event) => event.stopPropagation()}><button ref={closeButton} className="detail-close" type="button" onClick={() => setSelected(null)} aria-label="Detayı kapat"><X size={20} /></button><span className="card-kicker">İŞLEM DETAYI</span><h2 id="detail-title">{selected.type === 'In' ? 'Gelen transfer' : 'Giden transfer'}</h2><strong className="detail-amount">{selected.type === 'In' ? '+' : '−'}{formatMoney(selected.amount)}</strong><dl><div><dt>Karşı cüzdan</dt><dd>{selected.walletCode}</dd></div><div><dt>Açıklama</dt><dd>{selected.description}</dd></div><div><dt>Tarih</dt><dd>{formatDate(selected.createdAt)}</dd></div></dl></section></div>}</div>
}
