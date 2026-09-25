import { useEffect, useState } from 'react'

/** Tarayıcı çevrimdışı olduğunda işlemlerin gönderilemeyeceğini bildirir. */
export function NetworkStatusBanner() {
  const [online, setOnline] = useState(() => navigator.onLine)
  useEffect(() => {
    const update = () => setOnline(navigator.onLine)
    window.addEventListener('online', update)
    window.addEventListener('offline', update)
    return () => { window.removeEventListener('online', update); window.removeEventListener('offline', update) }
  }, [])
  return online ? null : <div className="network-banner" role="status">İnternet bağlantısı yok. Bağlantı gelene kadar yeni işlemler gönderilemez.</div>
}
