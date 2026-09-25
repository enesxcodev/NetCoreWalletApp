import { AtSign, Mail, UserRound } from 'lucide-react'
import type { InputHTMLAttributes } from 'react'
import type { UseFormRegisterReturn } from 'react-hook-form'

const icons = { user: UserRound, email: Mail, at: AtSign }

interface FormFieldProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string
  error?: string
  icon?: keyof typeof icons
  registration?: UseFormRegisterReturn
}

/** Form alanını ikon, erişilebilir hata ve ortak görsel stille sunar. */
export function FormField({ label, error, icon = 'user', id, registration, ...props }: FormFieldProps) {
  const Icon = icons[icon]
  const fieldId = id ?? registration?.name ?? props.name
  return (
    <div className="field-wrap">
      <label className="field-label" htmlFor={fieldId}>{label}</label>
      <div className={`input-shell ${error ? 'input-error' : ''}`}>
        <Icon className="input-icon" size={18} aria-hidden="true" />
        <input id={fieldId} autoComplete={props.autoComplete} type={props.type} placeholder={props.placeholder} aria-invalid={Boolean(error)} aria-describedby={error ? `${fieldId}-error` : undefined} {...registration} />
      </div>
      {error && <p className="field-error" id={`${fieldId}-error`}>{error}</p>}
    </div>
  )
}
