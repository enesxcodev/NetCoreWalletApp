import { Component, type ReactNode } from 'react'

interface ErrorBoundaryProps { children: ReactNode }
interface ErrorBoundaryState { hasError: boolean }

/** Beklenmeyen render hatalarında boş ekran yerine geri dönüş sunar. */
export class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  state: ErrorBoundaryState = { hasError: false }

  static getDerivedStateFromError(): ErrorBoundaryState { return { hasError: true } }

  componentDidCatch(): void {
    // Hata ileride merkezi izleme servisine bağlanabilir; hassas veri loglanmaz.
  }

  render() {
    if (this.state.hasError) return <main className="route-loading"><h1>Bir şeyler ters gitti</h1><p>Sayfayı yenileyerek tekrar deneyebilirsiniz.</p><button className="primary-button error-retry" onClick={() => window.location.reload()}>Sayfayı yenile</button></main>
    return this.props.children
  }
}
