import { Activity, ArrowRight, Database, Radio, Server, Workflow } from 'lucide-react'

const stages = [
  { title: 'MSSQL', label: 'Asıl kayıt', icon: Database, description: 'Bakiye, transfer ve olay kaydı aynı SQL transaction içinde yazılır.' },
  { title: 'Outbox', label: 'Güvenli bekleme', icon: Workflow, description: 'Mesaj SQL Outbox tablosunda tutulur; broker kapalı olsa da kaybolmaz.' },
  { title: 'RabbitMQ', label: 'Mesaj taşıma', icon: Radio, description: 'Outbox mesajı kuyruğa yayımlar; consumer arka planda alır.' },
  { title: 'MongoDB', label: 'Okuma modeli', icon: Server, description: 'Consumer işlem geçmişi belgesini ekler veya günceller.' },
  { title: 'Redis', label: 'Önbellek', icon: Activity, description: 'Consumer geçmiş önbelleğinin sürümünü artırır; sonraki okuma taze veriyi yükler.' },
]

/** Gerçek transferin SQL Outbox'tan raporlama ve önbelleğe uzanan yolunu açıklar. */
export function ArchitectureFlow() {
  return <ol className="architecture-flow">{stages.map(({ title, label, icon: Icon, description }, index) => <li className="architecture-node" key={title}><div className="architecture-node-head"><span className="architecture-node-icon"><Icon size={19} /></span><div><strong>{title}</strong><small>{label}</small></div></div><p>{description}</p>{index < stages.length - 1 && <ArrowRight className="architecture-arrow" size={18} aria-hidden="true" />}</li>)}</ol>
}
