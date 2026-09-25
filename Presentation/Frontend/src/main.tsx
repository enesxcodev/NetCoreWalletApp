import React from 'react'
import ReactDOM from 'react-dom/client'
import { AppProviders } from './app/app-providers'
import { AppRouter } from './app/app-router'
import { ErrorBoundary } from './app/error-boundary'
import './styles/global.css'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <ErrorBoundary>
      <AppProviders><AppRouter /></AppProviders>
    </ErrorBoundary>
  </React.StrictMode>,
)
