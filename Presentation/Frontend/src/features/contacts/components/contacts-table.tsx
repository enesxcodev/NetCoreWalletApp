import { UsersRound } from 'lucide-react'
import type { TransferContact } from '../api'
import { CopyWalletCodeButton } from './copy-wallet-code-button'

/** Transfer kişilerini masaüstünde tablo, mobilde kart olarak listeler. */
export function ContactsTable({ contacts, searched }: { contacts: TransferContact[]; searched: boolean }) {
  if (!contacts.length) return <div className="contacts-empty"><UsersRound size={27} /><strong>{searched ? 'Aramanızla eşleşen kişi bulunamadı' : 'Henüz transfer kişisi bulunmuyor'}</strong><p>{searched ? 'Farklı bir ad, kullanıcı adı veya e-posta deneyin.' : 'Cüzdanı olan kullanıcılar burada görünecek.'}</p></div>

  return <div className="contacts-table-wrap"><table className="contacts-table">
    <thead><tr><th>Ad soyad</th><th>Kullanıcı adı</th><th>Cüzdan kodu</th><th><span className="sr-only">İşlem</span></th></tr></thead>
    <tbody>{contacts.map((contact) => <tr key={contact.walletCode}>
      <td data-label="Ad soyad"><span className="contact-person"><span className="contact-avatar" aria-hidden="true">{contact.firstName[0]}{contact.lastName[0]}</span><strong>{contact.firstName} {contact.lastName}</strong></span></td>
      <td data-label="Kullanıcı adı"><span className="contact-username">@{contact.userName}</span></td>
      <td data-label="Cüzdan kodu"><code className="contact-wallet-code">{contact.walletCode}</code></td>
      <td data-label="Kopyala"><CopyWalletCodeButton code={contact.walletCode} /></td>
    </tr>)}</tbody>
  </table></div>
}
