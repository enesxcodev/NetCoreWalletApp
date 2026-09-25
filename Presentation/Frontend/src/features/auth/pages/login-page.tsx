import { useLocation } from 'react-router-dom'
import { AuthLayout } from '../../../layouts/auth-layout'
import { LoginForm } from '../components/login-form'

/** Oturum açma ekranını ortak auth görünümü içinde sunar. */
export function LoginPage() {
  const location = useLocation()
  const notice = (location.state as { notice?: string } | null)?.notice
  return <AuthLayout page="login"><>{notice && <p className="form-success" role="status">{notice}</p>}<LoginForm /></></AuthLayout>
}
