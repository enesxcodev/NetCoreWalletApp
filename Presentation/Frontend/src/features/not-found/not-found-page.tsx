import { Link } from 'react-router-dom'

/** Eşleşmeyen adresler için ana sayfaya dönüş bağlantısı verir. */
export function NotFoundPage() {
  return <main className="not-found-page"><span className="not-found-code">404</span><h1>Bu sayfayı bulamadık</h1><p>Bağlantı değişmiş olabilir veya adresi yanlış yazmış olabilirsiniz.</p><Link className="primary-button not-found-button" to="/login">Giriş sayfasına dön <span aria-hidden="true">→</span></Link></main>
}
