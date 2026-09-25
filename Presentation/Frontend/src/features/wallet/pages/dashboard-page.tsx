import { ArrowDownLeft, ArrowUpRight, Copy, Eye, EyeOff, History, WalletCards } from 'lucide-react'
import { useState } from 'react'
import { Link } from 'react-router-dom'
import { QueryError, QuerySkeleton } from '../../../components/query-state'
import { formatMoney } from '../../../lib/format'
import { TransactionList } from '../../transactions/components/transaction-list'
import { useTransactions } from '../../transactions/hooks'
import { useWallet } from '../hooks'

/** Güncel bakiye, cüzdan kodu, hızlı işlemler ve son transferleri gösterir. */
export function DashboardPage() {
  const wallet = useWallet()
  const recent = useTransactions(1, 5)
  const [balanceVisible, setBalanceVisible] = useState(true)
  const [copied, setCopied] = useState(false)
  const copyCode = async () => {
    if (!wallet.data) return
    await navigator.clipboard.writeText(wallet.data.code)
    setCopied(true)
    window.setTimeout(() => setCopied(false), 2000)
  }
  return <div className="content-stack">
    <header className="page-heading"><span className="card-kicker">GENEL BAKIŞ</span><h1>Cüzdanınız, tek ekranda.</h1><p>Bakiyenizi ve son hareketlerinizi buradan takip edin.</p></header>
    {wallet.isPending ? <QuerySkeleton /> : wallet.isError ? <QueryError retry={() => void wallet.refetch()} /> : <div className="overview-grid">
      <section className="balance-card"><div className="balance-top"><span>TOPLAM BAKİYE</span><WalletCards size={24} /></div><div className="balance-number">{balanceVisible ? formatMoney(wallet.data.balance) : '••••••••'} <button type="button" onClick={() => setBalanceVisible(!balanceVisible)} aria-label={balanceVisible ? 'Bakiyeyi gizle' : 'Bakiyeyi göster'}>{balanceVisible ? <EyeOff size={20} /> : <Eye size={20} />}</button></div><p>Güncel kullanılabilir bakiyeniz</p><div className="balance-decor" /></section>
      <section className="wallet-code-card"><span className="card-kicker">CÜZDAN KODUNUZ</span><h2>Ödeme almak artık kolay.</h2><p>Kodunuzu paylaşarak transfer alabilirsiniz.</p><div className="wallet-code-line"><code>{wallet.data.code}</code><button type="button" onClick={() => void copyCode()} aria-label="Cüzdan kodunu kopyala"><Copy size={18} /></button></div><small role="status">{copied ? 'Kod kopyalandı' : 'Kodu güvenle paylaşabilirsiniz'}</small></section>
    </div>}
    <section className="content-section"><div className="section-heading"><div><span className="card-kicker">HIZLI ERİŞİM</span><h2>Ne yapmak istersiniz?</h2></div></div><div className="quick-actions"><Link to="/deposit"><span className="action-icon mint"><ArrowDownLeft /></span><strong>Para yatır</strong><small>Bakiyenizi artırın</small></Link><Link to="/withdraw"><span className="action-icon coral"><ArrowUpRight /></span><strong>Para çek</strong><small>Bakiyenizi kullanın</small></Link><Link to="/transfer"><span className="action-icon lilac"><ArrowUpRight /></span><strong>Transfer yap</strong><small>Kolayca para gönderin</small></Link></div></section>
    <section className="content-section"><div className="section-heading"><div><span className="card-kicker">HESAP HAREKETLERİ</span><h2>Son transferler</h2></div><Link to="/transactions">Tümünü gör <History size={15} /></Link></div>{recent.isPending ? <QuerySkeleton /> : recent.isError ? <QueryError retry={() => void recent.refetch()} /> : <TransactionList items={recent.data} />}</section>
  </div>
}
