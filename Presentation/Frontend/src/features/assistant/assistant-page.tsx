import { Bot, MessageCircle, Plus, Send, Sparkles, UserRound } from 'lucide-react'
import { useEffect, useRef, useState, type FormEvent } from 'react'
import { ApiError } from '../../types/api'
import { sendAssistantMessage, type TransferContact } from './api'

type ChatMessage = {
  id: string
  role: 'assistant' | 'user'
  text: string
  contacts?: TransferContact[] | null
}

const suggestions = ['Bakiyem ne kadar?', 'Ahmet Yılmaz’ı bul']

/** Cüzdanla ilgili soruları asistan API'sine gönderir. */
export function AssistantPage() {
  const [messages, setMessages] = useState<ChatMessage[]>([
    {
      id: 'welcome',
      role: 'assistant',
      text: 'Merhaba! Bakiyeni öğrenebilir, cüzdanı olan kullanıcıları ad, kullanıcı adı veya e-posta ile arayabilirim. Nasıl yardımcı olayım?',
    },
  ])
  const [draft, setDraft] = useState('')
  const [isSending, setIsSending] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const conversationEndRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    conversationEndRef.current?.scrollIntoView({ behavior: 'smooth', block: 'end' })
  }, [messages, isSending])

  async function sendMessage(rawMessage: string) {
    const message = rawMessage.trim()
    if (!message || isSending) return

    setMessages((current) => [...current, { id: crypto.randomUUID(), role: 'user', text: message }])
    setDraft('')
    setError(null)
    setIsSending(true)

    try {
      const response = await sendAssistantMessage(message)
      setMessages((current) => [...current, {
        id: crypto.randomUUID(),
        role: 'assistant',
        text: response.reply,
        contacts: response.contacts,
      }])
    } catch (cause) {
      setError(cause instanceof ApiError ? cause.message : 'Asistana ulaşılamadı. API bağlantını kontrol edip tekrar dene.')
    } finally {
      setIsSending(false)
    }
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    void sendMessage(draft)
  }

  function startNewChat() {
    setMessages([{
      id: 'welcome',
      role: 'assistant',
      text: 'Merhaba! Bakiyeni öğrenebilir, cüzdanı olan kullanıcıları ad, kullanıcı adı veya e-posta ile arayabilirim. Nasıl yardımcı olayım?',
    }])
    setDraft('')
    setError(null)
  }

  return (
    <div className="content-stack assistant-page">
      <header className="page-heading assistant-heading">
        <div>
          <span className="card-kicker">WALLET ASİSTANI</span>
          <h1>Cüzdanın hakkında sor</h1>
          <p>Bakiyeni ve cüzdan bilgilerini doğal dille öğren.</p>
        </div>
        <button className="assistant-new-chat" type="button" onClick={startNewChat} disabled={isSending}>
          <Plus size={16} /> Yeni sohbet
        </button>
      </header>

      <section className="assistant-chat-card" aria-label="Cüzdan asistanı sohbeti">
        <div className="assistant-chat-status">
          <span className="assistant-status-icon"><Sparkles size={17} /></span>
          <div><strong>Cüzdan asistanı</strong><small>Çevrimiçi · Salt okunur</small></div>
          <span className="assistant-readonly-badge">Bakiyeni değiştiremez</span>
        </div>

        <div className="assistant-messages" aria-live="polite" aria-relevant="additions text">
          {messages.map((message) => (
            <article className={`assistant-message ${message.role} ${message.contacts ? 'has-contacts' : ''}`} key={message.id}>
              <span className="assistant-message-avatar" aria-hidden="true">
                {message.role === 'assistant' ? <Bot size={17} /> : <UserRound size={16} />}
              </span>
              <div className="assistant-message-content">
                <small>{message.role === 'assistant' ? 'Asistan' : 'Sen'}</small>
                <p>{message.text}</p>
                {message.contacts && <ContactsTable contacts={message.contacts} />}
              </div>
            </article>
          ))}
          {isSending && (
            <article className="assistant-message assistant">
              <span className="assistant-message-avatar" aria-hidden="true"><Bot size={17} /></span>
              <div className="assistant-message-content"><small>Asistan</small><p className="assistant-thinking">Yanıt hazırlanıyor<span>…</span></p></div>
            </article>
          )}
          <div ref={conversationEndRef} />
        </div>

        {messages.length === 1 && (
          <div className="assistant-suggestions" aria-label="Örnek sorular">
            {suggestions.map((suggestion) => (
              <button key={suggestion} type="button" onClick={() => void sendMessage(suggestion)} disabled={isSending}>
                <MessageCircle size={14} /> {suggestion}
              </button>
            ))}
          </div>
        )}

        <form className="assistant-composer" onSubmit={handleSubmit}>
          {error && <p className="assistant-error" role="alert">{error}</p>}
          <label className="assistant-input-wrap">
            <span className="sr-only">Mesajın</span>
            <textarea
              value={draft}
              onChange={(event) => setDraft(event.target.value)}
              onKeyDown={(event) => {
                if (event.key === 'Enter' && !event.shiftKey) {
                  event.preventDefault()
                  void sendMessage(draft)
                }
              }}
              placeholder="Örneğin: Bakiyem ne kadar? veya ayşe@example.com adresini ara"
              rows={1}
              maxLength={1000}
              disabled={isSending}
            />
            <button className="assistant-send-button" type="submit" aria-label="Mesajı gönder" disabled={!draft.trim() || isSending}>
              {isSending ? <span className="spinner assistant-spinner" /> : <Send size={17} />}
            </button>
          </label>
          <p className="assistant-composer-hint">Göndermek için Enter · yeni satır için Shift + Enter</p>
        </form>
      </section>
    </div>
  )
}

function ContactsTable({ contacts }: { contacts: TransferContact[] }) {
  if (contacts.length === 0) {
    return <p className="assistant-contact-empty">Bu aramayla eşleşen cüzdan bulunamadı.</p>
  }

  return (
    <div className="assistant-contact-table-wrap">
      <table className="assistant-contact-table">
        <caption className="sr-only">Aramayla eşleşen cüzdan sahipleri</caption>
        <thead>
          <tr><th>Ad soyad</th><th>Kullanıcı adı</th><th>Cüzdan kodu</th></tr>
        </thead>
        <tbody>
          {contacts.map((contact) => (
            <tr key={contact.walletCode}>
              <td>{contact.firstName} {contact.lastName}</td>
              <td>@{contact.userName}</td>
              <td><span className="assistant-wallet-code">{contact.walletCode}</span></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
