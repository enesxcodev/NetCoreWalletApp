import { AuthLayout } from '../../../layouts/auth-layout'
import { RegisterForm } from '../components/register-form'

/** Yeni kullanıcı kayıt ekranını ortak auth görünümüyle gösterir. */
export function RegisterPage() {
  return <AuthLayout page="register"><RegisterForm /></AuthLayout>
}
