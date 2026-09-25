import { HeroUIProvider } from '@heroui/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useState, type PropsWithChildren } from 'react'
import { BrowserRouter } from 'react-router-dom'
import { AuthProvider } from './auth-context'
import { ToastProvider } from '../components/toast-provider'

/** UI, sunucu state'i, oturum ve yönlendirme sağlayıcılarını bir araya getirir. */
export function AppProviders({ children }: PropsWithChildren) {
  const [queryClient] = useState(() => new QueryClient({
    defaultOptions: { queries: { retry: 1, refetchOnWindowFocus: false }, mutations: { retry: false } },
  }))

  return <HeroUIProvider><QueryClientProvider client={queryClient}><AuthProvider><BrowserRouter><ToastProvider>{children}</ToastProvider></BrowserRouter></AuthProvider></QueryClientProvider></HeroUIProvider>
}
