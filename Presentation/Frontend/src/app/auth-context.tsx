import { createContext, useContext, useEffect, useMemo, useState, type PropsWithChildren } from 'react'
import { authToken } from '../lib/auth-token'
import { useQueryClient } from '@tanstack/react-query'

interface AuthContextValue {
  token: string | null
  isLoading: boolean
  signIn: (token: string) => void
  signOut: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

/** Oturum bilgisini route guard'ları ve auth sayfaları arasında paylaşır. */
export function AuthProvider({ children }: PropsWithChildren) {
  const queryClient = useQueryClient()
  const [token, setToken] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    setToken(authToken.get())
    setIsLoading(false)
    const onUnauthorized = () => { queryClient.clear(); setToken(null) }
    window.addEventListener('wallet:unauthorized', onUnauthorized)
    return () => window.removeEventListener('wallet:unauthorized', onUnauthorized)
  }, [queryClient])

  const value = useMemo<AuthContextValue>(
    () => ({
      token,
      isLoading,
      signIn: (nextToken) => {
        queryClient.clear()
        authToken.set(nextToken)
        setToken(nextToken)
      },
      signOut: () => {
        queryClient.clear()
        authToken.clear()
        setToken(null)
      },
    }),
    [token, isLoading, queryClient],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

/** Sağlayıcı dışındaki kullanım hatalarını erken bildirerek oturum erişimi sağlar. */
// eslint-disable-next-line react-refresh/only-export-components
export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext)
  if (!context) throw new Error('useAuth, AuthProvider içinde kullanılmalıdır.')
  return context
}
