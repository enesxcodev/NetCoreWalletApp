import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { Link, useNavigate } from 'react-router-dom'
import { FormField } from './form-field'
import { PasswordField } from '../../../components/password-field'
import { errorMessage } from '../../../lib/error-message'
import { registerSchema } from '../schemas/auth-schemas'
import type { z } from 'zod'
import { useRegisterMutation } from '../hooks/use-auth-mutations'

type RegisterValues = z.infer<typeof registerSchema>

/** Kayıt alanlarını doğrular ve başarılı hesap açılışını kullanıcıya bildirir. */
export function RegisterForm() {
  const navigate = useNavigate()
  const mutation = useRegisterMutation()
  const { register, handleSubmit, formState: { errors } } = useForm<RegisterValues>({ resolver: zodResolver(registerSchema), mode: 'onBlur' })

  const onSubmit = handleSubmit(async (values) => {
    try {
      await mutation.mutateAsync(values)
      navigate('/login', { replace: true, state: { notice: 'Hesabınız oluşturuldu. Şimdi giriş yapabilirsiniz.' } })
    } catch {
      // Mutation hatası form üzerinde gösterilir.
    }
  })
  const serverError = mutation.error ? errorMessage(mutation.error) : null

  return (
    <div className="auth-card register-card">
      <div className="card-heading">
        <span className="card-kicker">WALLET AİLESİNE KATILIN</span>
        <h2>Hesabınızı oluşturun</h2>
        <p>Güvenli cüzdanınız birkaç adımda hazır.</p>
      </div>
      <form className="auth-form register-form" onSubmit={onSubmit} noValidate>
        {serverError && <div className="form-alert" role="alert">{serverError}</div>}
        <div className="field-row">
          <FormField label="Ad" placeholder="Adınız" autoComplete="given-name" registration={register('firstName')} error={errors.firstName?.message} />
          <FormField label="Soyad" placeholder="Soyadınız" autoComplete="family-name" registration={register('lastName')} error={errors.lastName?.message} />
        </div>
        <FormField label="E-posta" placeholder="ornek@posta.com" autoComplete="email" type="email" icon="email" registration={register('email')} error={errors.email?.message} />
        <FormField label="Kullanıcı adı" placeholder="Kullanıcı adınızı seçin" autoComplete="username" icon="at" registration={register('userName')} error={errors.userName?.message} />
        <PasswordField label="Şifre" placeholder="En az 6 karakter" autoComplete="new-password" registration={register('password')} error={errors.password?.message} />
        <p className="password-hint"><span className="hint-check">✓</span> En az 6 karakter kullanın.</p>
        <button className="primary-button" type="submit" disabled={mutation.isPending}>
          {mutation.isPending ? <><span className="spinner" /> Hesap oluşturuluyor</> : <>Ücretsiz hesap oluştur <span aria-hidden="true">→</span></>}
        </button>
      </form>
      <p className="auth-switch">Zaten hesabınız var mı? <Link to="/login">Giriş yapın <span aria-hidden="true">→</span></Link></p>
    </div>
  )
}
