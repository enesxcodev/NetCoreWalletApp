import { Activity, Database, Radio, Server, Workflow } from 'lucide-react'
import { ArchitectureFlow } from './architecture-flow'
import { TransferTimeline } from './transfer-timeline'

const services = [
  { title: 'MSSQL + Outbox', description: 'Transfer ve yayınlanacak olay aynı kalıcı transaction içinde.', icon: Database, kind: 'durable' },
  { title: 'RabbitMQ', description: 'Olayları producer ile consumer arasında taşır.', icon: Radio, kind: 'broker' },
  { title: 'MongoDB', description: 'İşlem geçmişi için okuma modelini saklar.', icon: Server, kind: 'read' },
  { title: 'Redis', description: 'Geçmiş yanıtlarını önbelleğe alır; sürüm değişince eski cache atlanır.', icon: Activity, kind: 'cache' },
]

/** Event-driven backend akışını, servis rollerini ve kesinti senaryosunu öğretir. */
export function ArchitecturePage() {
  return <div className="content-stack architecture-page"><header className="page-heading"><span className="card-kicker">SİSTEM TASARIMI</span><h1>Transferin arka plandaki yolculuğu</h1><p>Kalıcı kayıt ile raporlama işlemlerinin nasıl güvenli biçimde ayrıldığını keşfedin.</p></header>
    <section className="architecture-intro"><div className="architecture-intro-icon"><Workflow size={23} /></div><div><strong>Önce güvenli kayıt, sonra arka plan işleri</strong><p>API önce bakiye ve transferi SQL’de kaydeder. Outbox, aynı transaction içindeki olayı saklar; mesajlaşma ve raporlama adımları ardından tamamlanır.</p></div></section>
    <section className="content-section"><div className="section-heading"><div><span className="card-kicker">VERİ AKIŞI</span><h2>MSSQL → Outbox → RabbitMQ → MongoDB → Redis</h2><p>Her bileşenin sorumluluğu farklıdır.</p></div></div><ArchitectureFlow /></section>
    <section className="service-grid" aria-label="Servis görevleri ve gözlem durumu">{services.map(({ title, description, icon: Icon, kind }) => <article className="service-card" key={title}><span className={`service-icon ${kind}`}><Icon size={18} /></span><div><strong>{title}</strong><p>{description}</p><small className="service-monitoring">Canlı sağlık ölçümü yok</small></div></article>)}</section>
    <TransferTimeline />
    <p className="architecture-disclaimer">Bu ekran canlı sağlık kontrolü değildir. Simülatör hiçbir API çağrısı yapmaz ve cüzdan bakiyesini değiştirmez.</p>
  </div>
}
