import { WalletCards } from 'lucide-react'

/** Wallet adını giriş ve kayıt ekranlarında ortak işaretle gösterir. */
export function Brand() {
  return (
    <a className="brand" href="/login" aria-label="Wallet ana sayfa">
      <span className="brand-mark"><WalletCards size={23} strokeWidth={2.4} /></span>
      <span><strong>Wallet</strong><small>Dijital Cüzdan</small></span>
    </a>
  )
}
