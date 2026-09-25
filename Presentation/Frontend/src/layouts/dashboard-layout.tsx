import { Activity, ArrowDownLeft, ArrowRightFromLine, ArrowUpRight, History, LayoutDashboard, Menu, Send, UsersRound, WalletCards } from 'lucide-react'
import { useEffect, useState } from 'react'
import { NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../app/auth-context'
import { NetworkStatusBanner } from '../components/network-status-banner'

/** Korumalı sayfalar için responsive gezinti ve içerik alanı sağlar. */
export function DashboardLayout() {
  const { signOut } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const [menuOpen, setMenuOpen] = useState(false)
  const logout = () => { signOut(); navigate('/login', { replace: true }) }
  const pageTitle: Record<string, string> = {
    '/dashboard': 'Genel bakış', '/deposit': 'Para yatır', '/withdraw': 'Para çek',
    '/transfer': 'Transfer', '/transactions': 'İşlem geçmişi', '/contacts': 'Transfer Kişileri', '/architecture': 'Mimari akış',
  }
  useEffect(() => { setMenuOpen(false) }, [location.pathname])

  return (
    <div className="dashboard-shell">
      <aside id="dashboard-navigation" className={`dashboard-sidebar ${menuOpen ? 'sidebar-open' : ''}`} aria-label="Ana menü">
        <a className="brand dashboard-brand" href="/dashboard"><span className="brand-mark"><WalletCards size={22} /></span><span><strong>Wallet</strong><small>Dijital Cüzdan</small></span></a>
        <p className="nav-caption">MENÜ</p>
        <NavLink className={({ isActive }) => `dashboard-nav-link ${isActive ? 'active' : ''}`} to="/dashboard" onClick={() => setMenuOpen(false)}><LayoutDashboard size={18} /> Genel bakış</NavLink>
        <NavLink className={({ isActive }) => `dashboard-nav-link ${isActive ? 'active' : ''}`} to="/deposit" onClick={() => setMenuOpen(false)}><ArrowDownLeft size={18} /> Para yatır</NavLink>
        <NavLink className={({ isActive }) => `dashboard-nav-link ${isActive ? 'active' : ''}`} to="/withdraw" onClick={() => setMenuOpen(false)}><ArrowUpRight size={18} /> Para çek</NavLink>
        <NavLink className={({ isActive }) => `dashboard-nav-link ${isActive ? 'active' : ''}`} to="/transfer" onClick={() => setMenuOpen(false)}><Send size={18} /> Transfer</NavLink>
        <NavLink className={({ isActive }) => `dashboard-nav-link ${isActive ? 'active' : ''}`} to="/transactions" onClick={() => setMenuOpen(false)}><History size={18} /> İşlem geçmişi</NavLink>
        <NavLink className={({ isActive }) => `dashboard-nav-link ${isActive ? 'active' : ''}`} to="/contacts" onClick={() => setMenuOpen(false)}><UsersRound size={18} /> Transfer Kişileri</NavLink>
        <NavLink className={({ isActive }) => `dashboard-nav-link ${isActive ? 'active' : ''}`} to="/architecture" onClick={() => setMenuOpen(false)}><Activity size={18} /> Mimari akış</NavLink>
        <div className="sidebar-bottom"><button className="logout-button" type="button" onClick={logout}><ArrowRightFromLine size={18} /> Güvenli çıkış</button><p>Wallet · Güvenli cüzdanınız</p></div>
      </aside>
      <div className="dashboard-main">
        <header className="dashboard-topbar"><button className="menu-toggle" type="button" aria-controls="dashboard-navigation" aria-expanded={menuOpen} aria-label={menuOpen ? 'Menüyü kapat' : 'Menüyü aç'} onClick={() => setMenuOpen((open) => !open)}><Menu size={21} /></button><span>Wallet <span className="breadcrumb-separator">/</span> <strong>{pageTitle[location.pathname] ?? 'Sayfa'}</strong></span><span className="secure-badge"><span /> Güvenli oturum</span></header>
        <NetworkStatusBanner />
        <main className="dashboard-content"><Outlet /></main>
      </div>
    </div>
  )
}
