import { Search, X } from 'lucide-react'

interface ContactSearchProps {
  value: string
  onChange: (value: string) => void
}

/** Ad soyad, kullanıcı adı veya e-postayla alıcı arama alanını sunar. */
export function ContactSearch({ value, onChange }: ContactSearchProps) {
  return <div className="contact-search">
    <label htmlFor="contact-search">Transfer kişisi ara</label>
    <div className="contact-search-input">
      <Search size={19} aria-hidden="true" />
      <input id="contact-search" type="search" value={value} maxLength={100} onChange={(event) => onChange(event.target.value)} placeholder="Ad soyad, kullanıcı adı veya e-posta" autoComplete="off" />
      {value && <button type="button" onClick={() => onChange('')} aria-label="Aramayı temizle"><X size={17} /></button>}
    </div>
    <p>Bir kişiyi bulmak için adı, kullanıcı adı veya e-posta adresini yazın.</p>
  </div>
}
