import { Check, Copy } from 'lucide-react'
import { useState } from 'react'

/** Seçilen kişinin cüzdan kodunu panoya kopyalar ve sonucu bildirir. */
export function CopyWalletCodeButton({ code }: { code: string }) {
  const [state, setState] = useState<'idle' | 'copied' | 'error'>('idle')

  const copyCode = async () => {
    try {
      await navigator.clipboard.writeText(code)
      setState('copied')
    } catch {
      setState('error')
    }
  }

  return <span className="copy-contact-wrap">
    <button className="copy-contact-button" type="button" onClick={() => void copyCode()} aria-label={`${code} cüzdan kodunu kopyala`}>
      {state === 'copied' ? <Check size={16} /> : <Copy size={16} />}
      <span>{state === 'copied' ? 'Kopyalandı' : 'Kopyala'}</span>
    </button>
    {state === 'error' && <small role="alert">Kopyalanamadı</small>}
  </span>
}
