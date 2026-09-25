import { ArrowDownLeft, ArrowUpRight } from 'lucide-react'
import { formatDate, formatMoney } from '../../../lib/format'
import type { TransactionItem } from '../api'

/** Gelen ve giden işlemleri metin, ikon ve tutarla açık biçimde listeler. */
export function TransactionList({ items, onSelect }: { items: TransactionItem[]; onSelect?: (item: TransactionItem) => void }) {
  if (!items.length) return <div className="empty-state">Henüz işlem yok. İlk transferiniz burada görünecek.</div>
  return <div className="transaction-list">{items.map((item, index) => {
    const incoming = item.type === 'In'
    return <button className="transaction-row" type="button" key={`${item.createdAt}-${item.walletCode}-${index}`} onClick={() => onSelect?.(item)}>
      <span className={`transaction-symbol ${incoming ? 'incoming' : 'outgoing'}`}>{incoming ? <ArrowDownLeft size={19} /> : <ArrowUpRight size={19} />}</span>
      <span className="transaction-main"><strong>{incoming ? 'Gelen transfer' : 'Giden transfer'}</strong><small>{item.walletCode} · {item.description}</small></span>
      <span className="transaction-side"><strong className={incoming ? 'positive' : ''}>{incoming ? '+' : '−'}{formatMoney(item.amount)}</strong><small>{formatDate(item.createdAt)}</small></span>
    </button>
  })}</div>
}
