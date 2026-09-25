import { ArrowDownLeft, ArrowUpRight, ChartNoAxesCombined, ShieldCheck, Zap } from 'lucide-react'
import type { PropsWithChildren } from 'react'
import phoneHero from '../assets/phone-hero.svg'
import walletHero from '../assets/wallet-hero.svg'
import { Brand } from '../components/brand'

interface AuthLayoutProps extends PropsWithChildren {
  page: 'login' | 'register'
}

/** Auth sayfalarına ortak marka, illüstrasyon ve mobil düzeni sağlar. */
export function AuthLayout({ page, children }: AuthLayoutProps) {
  const isLogin = page === 'login'
  const items = isLogin
    ? [
        { icon: <Zap size={19} />, title: 'Hızlı işlemler', text: 'Günlük işlemlerinizi kolayca yönetin.' },
        { icon: <ShieldCheck size={19} />, title: 'Güvenli altyapı', text: 'Hesabınız koruma altında.' },
        { icon: <ChartNoAxesCombined size={19} />, title: 'Finansal kontrol', text: 'Hareketlerinizi tek yerde görün.' },
      ]
    : [
        { icon: <ArrowUpRight size={19} />, title: 'Kolay başlangıç', text: 'Birkaç adımda hesabınız hazır.' },
        { icon: <ShieldCheck size={19} />, title: 'Güvenli hesap', text: 'Bilgileriniz güvenle korunur.' },
        { icon: <ArrowDownLeft size={19} />, title: 'Modern deneyim', text: 'Cüzdanınızı dilediğiniz yerden yönetin.' },
      ]

  return (
    <main className={`auth-page ${isLogin ? 'auth-login' : 'auth-register'}`}>
      <section className="auth-visual" aria-label="Wallet özellikleri">
        <Brand />
        <div className="visual-copy">
          <p className="eyebrow">PARANIZ, KONTROLÜNÜZDE</p>
          <h1>{isLogin ? <>Finansınızı<br />tek yerden yönetin.</> : <>Cüzdanınızı<br />kolayca oluşturun.</>}</h1>
          <p className="visual-description">Güvenli, hızlı ve modern bir dijital cüzdan deneyimiyle tanışın.</p>
        </div>
        <img className="hero-illustration" src={isLogin ? walletHero : phoneHero} alt={isLogin ? 'Kartlarıyla birlikte dijital cüzdan illüstrasyonu' : 'Bakiye grafiği ve hareketleri gösteren telefon illüstrasyonu'} />
        <div className="feature-list">
          {items.map((item) => <div className="feature-item" key={item.title}><span className="feature-icon">{item.icon}</span><span><strong>{item.title}</strong><small>{item.text}</small></span></div>)}
        </div>
        <p className="visual-footnote">Günlük finansınız için sade ve güvenli bir başlangıç.</p>
      </section>
      <section className="auth-form-side">
        <div className="mobile-brand"><Brand /></div>
        {children}
        <p className="form-side-footnote"><ShieldCheck size={14} /> Güvenli bağlantı · Bilgileriniz şifrelenir</p>
      </section>
    </main>
  )
}
