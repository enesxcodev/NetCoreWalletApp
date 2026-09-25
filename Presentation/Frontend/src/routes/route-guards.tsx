import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from '../app/auth-context'

/** Oturum durumuna göre bekler veya çocuk route'a yönlendirir. */
function RouteLoading() {
  return <main className="route-loading"><span className="spinner" /><p>Oturum kontrol ediliyor</p></main>
}

/** Giriş yapılmamış kullanıcıyı korumalı içerikten uzak tutar. */
export function ProtectedRoute() {
  const { token, isLoading } = useAuth()
  const location = useLocation()
  if (isLoading) return <RouteLoading />
  return token ? <Outlet /> : <Navigate to="/login" replace state={{ from: location.pathname }} />
}

/** Açık auth ekranlarını oturum açmış kullanıcıdan gizler. */
export function PublicOnlyRoute() {
  const { token, isLoading } = useAuth()
  if (isLoading) return <RouteLoading />
  return token ? <Navigate to="/dashboard" replace /> : <Outlet />
}
