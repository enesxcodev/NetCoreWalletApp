import { useState } from 'react'
import { Activity, Check, Circle, Pause, Play, RotateCcw } from 'lucide-react'

const steps = [
  { key: 'sql', title: 'SQL transaction tamamlandı', detail: 'Bakiye, transfer ve Outbox olayı birlikte commit edildi.' },
  { key: 'outbox', title: 'Outbox mesajı hazır', detail: 'Broker bağlantısı gelene kadar olay SQL’de dayanıklı biçimde bekler.' },
  { key: 'rabbit', title: 'RabbitMQ mesajı aldı', detail: 'Mesaj kuyruğa yayımlandı; consumer teslim alabilir.' },
  { key: 'mongo', title: 'MongoDB geçmişi güncellendi', detail: 'Transfer okuma modeli idempotent biçimde yazıldı.' },
  { key: 'redis', title: 'Redis önbelleği yenilemeye hazır', detail: 'Geçmiş önbelleği sürümü artırıldı; sonraki okuma güncel veriyi getirir.' },
]

/** Para göndermeden, Outbox ve broker kesintisi senaryosunu eğitim için canlandırır. */
export function TransferTimeline() {
  const [started, setStarted] = useState(false)
  const [brokerAvailable, setBrokerAvailable] = useState(true)
  const blocked = started && !brokerAvailable
  const completed = started && brokerAvailable
  const status = (index: number) => !started ? 'pending' : blocked ? (index < 2 ? 'complete' : index === 2 ? 'blocked' : 'pending') : 'complete'
  return <section className="content-section timeline-card" aria-labelledby="timeline-title">
    <div className="section-heading"><div><span className="card-kicker">ETKİLEŞİMLİ ÖRNEK</span><h2 id="timeline-title">Transfer olayı adım adım</h2><p>Bu senaryo eğitim simülasyonudur; gerçek transfer veya canlı servis kontrolü yapmaz.</p></div></div>
    <div className={`simulation-notice ${blocked ? 'is-blocked' : ''}`} role="status">{!started ? 'Akış henüz başlatılmadı.' : blocked ? 'RabbitMQ simülasyonda kapalı. Olay Outbox’ta bekliyor; SQL transferi tamamlandı.' : 'Simülasyonda mesaj MongoDB’ye ulaştı ve Redis geçmiş sürümü artırıldı.'}</div>
    <ol className="event-timeline">{steps.map((step, index) => { const state = status(index); const Icon = state === 'complete' ? Check : state === 'blocked' ? Pause : Circle; return <li className={`event-step ${state}`} key={step.key}><span className="event-step-icon"><Icon size={16} /></span><div><strong>{step.title}</strong><p>{step.detail}</p></div></li> })}</ol>
    <div className="simulation-actions"><button type="button" className="primary-button" onClick={() => setStarted(true)} disabled={completed}><Play size={15} /> Akışı başlat</button><button type="button" className="secondary-button" onClick={() => { setStarted(true); setBrokerAvailable(false) }} disabled={blocked}><Pause size={15} /> RabbitMQ kesintisini simüle et</button><button type="button" className="secondary-button" onClick={() => setBrokerAvailable(true)} disabled={!started || brokerAvailable}><Activity size={15} aria-hidden="true" /> Bağlantıyı geri getir</button><button type="button" className="text-button" onClick={() => { setStarted(false); setBrokerAvailable(true) }}><RotateCcw size={14} /> Sıfırla</button></div>
  </section>
}
