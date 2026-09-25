import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../../../app/auth-context'
import { PasswordField } from '../../../components/password-field'
import { ApiError } from '../../../types/api'
import { errorMessage } from '../../../lib/error-message'
import { loginSchema } from '../schemas/auth-schemas'
import type { z } from 'zod'
import { useLoginMutation } from '../hooks/use-auth-mutations'
import { FormField } from './form-field'

type LoginValues = z.infer<typeof loginSchema>

/** Giriş bilgilerini doğrular, oturum açar ve dashboard'a yönlendirir. */
export function LoginForm() {
  const navigate = useNavigate()
  const { signIn } = useAuth()
  const mutation = useLoginMutation()
  const { register, handleSubmit, formState: { errors } } = useForm<LoginValues>({ resolver: zodResolver(loginSchema), mode: 'onBlur' })

  const onSubmit = handleSubmit(async (values) => {
    try {
      const token = await mutation.mutateAsync(values)
      if (!token) throw new ApiError('Sunucu oturum anahtarı döndürmedi.', 500)
      signIn(token)
      navigate('/dashboard', { replace: true })
    } catch {
      // Mutation hatası aşağıda güvenli ve kullanıcı dostu biçimde gösterilir.
    }
  })
  const serverError = mutation.error ? errorMessage(mutation.error) : null

  return (
    <div className="auth-card">
      <div className="card-heading">
        <span className="card-kicker">YENİDEN MERHABA</span>
        <h2>Tekrar hoş geldiniz</h2>
        <p>Hesabınıza giriş yaparak kaldığınız yerden devam edin.</p>
      </div>
      <form className="auth-form" onSubmit={onSubmit} noValidate>
        {serverError && <div className="form-alert" role="alert">{serverError}</div>}
        <FormField label="Kullanıcı adı" placeholder="Kullanıcı adınızı girin" autoComplete="username" registration={register('userName')} error={errors.userName?.message} />
        <PasswordField label="Şifre" placeholder="Şifrenizi girin" autoComplete="current-password" registration={register('password')} error={errors.password?.message} />
        <div className="form-meta"><span>Hesabınız güvende</span><span className="secure-dot" /></div>
        <button className="primary-button" type="submit" disabled={mutation.isPending}>
          {mutation.isPending ? <><span className="spinner" /> Giriş yapılıyor</> : <>Giriş yap <span aria-hidden="true">→</span></>}
        </button>
        <div className="form-divider"><span>veya</span></div>
        <button className="secondary-button" type="button" disabled title="GitHub ile giriş yakında eklenecek">
          <svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M12 .9a11.1 11.1 0 0 0-3.51 21.63c.55.1.76-.24.76-.54v-2.1c-3.1.67-3.75-1.31-3.75-1.31-.5-1.28-1.24-1.62-1.24-1.62-1.01-.7.08-.69.08-.69 1.12.08 1.7 1.15 1.7 1.15.99 1.7 2.6 1.21 3.24.92.1-.72.39-1.21.7-1.49-2.48-.28-5.09-1.24-5.09-5.52 0-1.22.44-2.22 1.15-3-.11-.28-.5-1.42.11-2.96 0 0 .94-.3 3.06 1.15a10.6 10.6 0 0 1 5.57 0c2.13-1.45 3.06-1.15 3.06-1.15.61 1.54.23 2.68.12 2.96.72.78 1.15 1.78 1.15 3 0 4.3-2.61 5.24-5.1 5.51.4.34.75 1.02.75 2.06v3.09c0 .3.2.65.77.54A11.1 11.1 0 0 0 12 .9Z"/></svg>
          GitHub ile giriş yap <span className="soon-label">Yakında</span>
        </button>
      </form>
      <p className="auth-switch">Henüz hesabınız yok mu? <Link to="/register">Hesap oluşturun <span aria-hidden="true">→</span></Link></p>
    </div>
  )
}
