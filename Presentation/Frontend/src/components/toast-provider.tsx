import { createContext, useCallback, useContext, useMemo, useState, type PropsWithChildren } from 'react'
import { CheckCircle2, CircleAlert, X } from 'lucide-react'

type ToastKind = 'success' | 'error'
type ToastItem = { id: number; message: string; kind: ToastKind }
type ToastApi = { showToast: (message: string, kind?: ToastKind) => void }
const ToastContext = createContext<ToastApi | null>(null)

/** Kısa başarı ve hata bildirimlerini erişilebilir biçimde uygulama çapında sunar. */
export function ToastProvider({ children }: PropsWithChildren) {
  const [items, setItems] = useState<ToastItem[]>([])
  const showToast = useCallback((message: string, kind: ToastKind = 'success') => {
    const id = Date.now() + Math.random()
    setItems((current) => [...current.slice(-2), { id, message, kind }])
    window.setTimeout(() => setItems((current) => current.filter((item) => item.id !== id)), 4500)
  }, [])
  const value = useMemo(() => ({ showToast }), [showToast])
  return <ToastContext.Provider value={value}>{children}<div className="toast-region" aria-label="Bildirimler">{items.map((item) => <div className={`toast-item ${item.kind}`} key={item.id} role={item.kind === 'error' ? 'alert' : 'status'}><span>{item.kind === 'error' ? <CircleAlert size={18} /> : <CheckCircle2 size={18} />}</span><p>{item.message}</p><button type="button" aria-label="Bildirimi kapat" onClick={() => setItems((current) => current.filter((entry) => entry.id !== item.id))}><X size={15} /></button></div>)}</div></ToastContext.Provider>
}

/** Bildirim sağlayıcısını kullanan component'e kısa toast gönderme işlevi verir. */
// eslint-disable-next-line react-refresh/only-export-components
export function useToast() {
  const value = useContext(ToastContext)
  if (!value) throw new Error('useToast, ToastProvider içinde kullanılmalıdır.')
  return value
}
