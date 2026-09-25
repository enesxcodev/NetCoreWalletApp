import { zodResolver } from '@hookform/resolvers/zod'
import { ArrowDownLeft, ArrowUpRight } from 'lucide-react'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { errorMessage } from '../../../lib/error-message'
import { formatMoney } from '../../../lib/format'
import { positiveAmount } from '../../../lib/amount'
import { QueryError, QuerySkeleton } from '../../../components/query-state'
import { useWallet } from '../hooks'
import { useMoneyMutation, type MoneyAction } from '../mutations'

const amountSchema = z.object({ amount: positiveAmount })
type AmountValues = z.input<typeof amountSchema>

/** Para yatırma ve çekme formunu aynı kurallarla, ayrı endpoint'lere bağlar. */
export function MoneyPage({ action }: { action: MoneyAction }) {
  const isDeposit = action === 'deposit'
  const wallet = useWallet()
  const mutation = useMoneyMutation(action)
  const { register, handleSubmit, reset, formState: { errors } } = useForm<AmountValues, unknown, z.output<typeof amountSchema>>({ resolver: zodResolver(amountSchema) })
  const submit = handleSubmit(async ({ amount }) => {
    try { await mutation.mutateAsync(amount); reset() } catch { /* Hata aşağıda gösterilir. */ }
  })
  return <div className="content-stack narrow-content"><header className="page-heading"><span className="card-kicker">{isDeposit ? 'BAKİYE EKLE' : 'BAKİYE KULLAN'}</span><h1>{isDeposit ? 'Para yatırın' : 'Para çekin'}</h1><p>{isDeposit ? 'Cüzdanınıza istediğiniz tutarı ekleyin.' : 'Cüzdanınızdan çekmek istediğiniz tutarı girin.'}</p></header>
    {wallet.isPending ? <QuerySkeleton /> : wallet.isError ? <QueryError retry={() => void wallet.refetch()} /> : <div className="compact-balance"><span>Güncel bakiye</span><strong>{formatMoney(wallet.data.balance)}</strong></div>}
    <section className="form-panel"><div className={`large-action-icon ${isDeposit ? 'mint' : 'coral'}`}>{isDeposit ? <ArrowDownLeft /> : <ArrowUpRight />}</div><h2>{isDeposit ? 'Cüzdanınıza para ekleyin' : 'Cüzdanınızdan para çekin'}</h2><p>İşlem sonrası güncel bakiyeniz otomatik yenilenir.</p><form onSubmit={submit} noValidate>
      <label htmlFor="amount">Tutar (₺)</label><div className="amount-input"><span>₺</span><input id="amount" inputMode="decimal" placeholder="0,00" aria-invalid={Boolean(errors.amount)} {...register('amount')} /></div>{errors.amount && <small className="field-error">{errors.amount.message}</small>}
      {mutation.isError && <div role="alert" className="form-alert">{errorMessage(mutation.error)}</div>}
      {mutation.isSuccess && <div role="status" className="form-success">İşlem başarıyla tamamlandı.</div>}
      <button className="primary-button" type="submit" disabled={mutation.isPending}>{mutation.isPending ? 'İşleniyor...' : isDeposit ? 'Para yatır' : 'Para çek'}</button>
    </form></section>
  </div>
}
