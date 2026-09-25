import { useState } from 'react'
import { Eye, EyeOff, LockKeyhole } from 'lucide-react'
import type { InputHTMLAttributes } from 'react'
import type { UseFormRegisterReturn } from 'react-hook-form'

interface PasswordFieldProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string
  error?: string
  registration?: UseFormRegisterReturn
}

/** Şifreyi gizli gösterir ve kullanıcı isterse görünür yapar. */
export function PasswordField({ label, error, id, registration, ...props }: PasswordFieldProps) {
  const [visible, setVisible] = useState(false)
  const fieldId = id ?? 'password'
  return (
    <div className="field-wrap">
      <label className="field-label" htmlFor={fieldId}>{label}</label>
      <div className={`input-shell ${error ? 'input-error' : ''}`}>
        <LockKeyhole className="input-icon" size={18} aria-hidden="true" />
        <input id={fieldId} type={visible ? 'text' : 'password'} autoComplete={props.autoComplete ?? 'current-password'} placeholder={props.placeholder} aria-invalid={Boolean(error)} aria-describedby={error ? `${fieldId}-error` : undefined} {...registration} />
        <button className="visibility-toggle" type="button" onClick={() => setVisible((current) => !current)} aria-label={visible ? 'Şifreyi gizle' : 'Şifreyi göster'}>
          {visible ? <EyeOff size={18} /> : <Eye size={18} />}
        </button>
      </div>
      {error && <p className="field-error" id={`${fieldId}-error`}>{error}</p>}
    </div>
  )
}
