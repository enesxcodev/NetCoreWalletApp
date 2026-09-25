import { RefreshCw } from 'lucide-react'

/** Sorgu hatasını teknik ayrıntı göstermeden tekrar deneme aksiyonuyla sunar. */
export function QueryError({ retry }: { retry: () => void }) {
  return <div className="query-error" role="alert"><strong>Bilgiler yüklenemedi</strong><p>Bağlantıyı kontrol edip yeniden deneyin.</p><button type="button" onClick={retry}><RefreshCw size={15} /> Tekrar dene</button></div>
}

/** Verinin beklenmesi sırasında boş ekran yerine iskelet gösterir. */
export function QuerySkeleton() {
  return <div className="query-skeleton" aria-label="Bilgiler yükleniyor" aria-busy="true"><span /><span /><span /></div>
}
