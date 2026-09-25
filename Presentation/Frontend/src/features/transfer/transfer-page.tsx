import { zodResolver } from '@hookform/resolvers/zod'
import { ArrowRight, Send } from 'lucide-react'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { formatMoney } from '../../lib/format'
import { ApiError } from '../../types/api'
import { errorMessage } from '../../lib/error-message'
import { useWallet } from '../wallet/hooks'
import { useTransfer } from './hooks'
import { useToast } from '../../components/toast-provider'
import { transferSchema, type TransferValues } from './schema'

/** Transferi önce özetler; belirsiz ağ hatasında aynı idempotency anahtarıyla yeniden dener. */
export function TransferPage() {
  const wallet = useWallet()
  const transfer = useTransfer()
  const { showToast } = useToast()
  const [pending, setPending] = useState<{ values: TransferValues; key: string } | null>(null)
  const [transactionId, setTransactionId] = useState<string | null>(null)
  const { register, handleSubmit, reset, formState: { errors } } = useForm<import('zod').input<typeof transferSchema>, unknown, TransferValues>({ resolver: zodResolver(transferSchema), defaultValues: { description: '' } })
  const prepare = handleSubmit((values) => { transfer.reset(); setTransactionId(null); setPending({ values, key: crypto.randomUUID() }) })
  const confirm = async () => {
    if (!pending) return
    try { setTransactionId(await transfer.mutateAsync({ values: pending.values, idempotencyKey: pending.key })); setPending(null); reset(); showToast('Transfer başarıyla kaydedildi.') } catch { /* Aynı istek özeti ve UUID yeniden deneme için korunur. */ }
  }
  return <div className="content-stack narrow-content"><header className="page-heading"><span className="card-kicker">GÜVENLİ TRANSFER</span><h1>Para gönderin</h1><p>Alıcının cüzdan kodunu girin, tutarı kontrol edin ve onaylayın.</p></header>
    <section className="form-panel"><div className="large-action-icon lilac"><Send /></div><h2>Transfer bilgileri</h2><p>İşlem onaylanmadan önce bilgileri gözden geçirebilirsiniz.</p>
      {transactionId && <div role="status" className="transfer-result"><strong>Transfer kaydedildi</strong><span>İşlem no: {transactionId}</span></div>}
      {!pending ? <form onSubmit={prepare} noValidate><label htmlFor="walletCode">Alıcı cüzdan kodu</label><input id="walletCode" className="plain-input" placeholder="WLT-XXXXXXXX" aria-invalid={Boolean(errors.walletCode)} {...register('walletCode')} />{errors.walletCode && <small className="field-error">{errors.walletCode.message}</small>}
        <label htmlFor="transferAmount">Tutar (₺)</label><div className="amount-input"><span>₺</span><input id="transferAmount" inputMode="decimal" placeholder="0,00" aria-invalid={Boolean(errors.amount)} {...register('amount')} /></div>{errors.amount && <small className="field-error">{errors.amount.message}</small>}
        <label htmlFor="description">Açıklama <span className="optional-label">İsteğe bağlı</span></label><input id="description" className="plain-input" placeholder="Örn. kira ödemesi" {...register('description')} />{errors.description && <small className="field-error">{errors.description.message}</small>}
        <button className="primary-button" type="submit">Özeti görüntüle <ArrowRight size={17} /></button></form> : <div className="transfer-confirm"><h3>Transfer özeti</h3><div><span>Alıcı</span><strong>{pending.values.walletCode}</strong></div><div><span>Gönderilecek tutar</span><strong>{formatMoney(pending.values.amount)}</strong></div><div><span>Açıklama</span><strong>{pending.values.description || 'Transfer'}</strong></div><div><span>Mevcut bakiye</span><strong>{wallet.data ? formatMoney(wallet.data.balance) : 'Yükleniyor'}</strong></div>
          {transfer.isError && <div role="alert" className="form-alert">{errorMessage(transfer.error)}{transfer.error instanceof ApiError && transfer.error.status === 409 ? ' Farklı işlem için yeni transfer başlatın.' : ' Durum belirsizse aynı isteği yeniden deneyin.'}</div>}
          <button className="primary-button" type="button" disabled={transfer.isPending || (transfer.error instanceof ApiError && transfer.error.status === 409)} onClick={() => void confirm()}>{transfer.isPending ? 'Gönderiliyor...' : transfer.isError ? 'Aynı transferi yeniden dene' : 'Transferi onayla'}</button><button className="text-button" type="button" disabled={transfer.isPending} onClick={() => { setPending(null); transfer.reset() }}>Bilgileri düzenle</button>
        </div>}
    </section>
  </div>
}
