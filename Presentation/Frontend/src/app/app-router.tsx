import { Navigate, Route, Routes } from 'react-router-dom'
import { DashboardLayout } from '../layouts/dashboard-layout'
import { LoginPage } from '../features/auth/pages/login-page'
import { RegisterPage } from '../features/auth/pages/register-page'
import { DashboardPage } from '../features/wallet/pages/dashboard-page'
import { MoneyPage } from '../features/wallet/pages/money-page'
import { TransferPage } from '../features/transfer/transfer-page'
import { TransactionsPage } from '../features/transactions/transactions-page'
import { ContactsPage } from '../features/contacts/contacts-page'
import { ArchitecturePage } from '../features/architecture/architecture-page'
import { NotFoundPage } from '../features/not-found/not-found-page'
import { ProtectedRoute, PublicOnlyRoute } from '../routes/route-guards'

/** Uygulamanın açık, korumalı ve bulunamayan sayfalarını eşler. */
export function AppRouter() {
  return (
    <Routes>
      <Route element={<PublicOnlyRoute />}>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
      </Route>
      <Route element={<ProtectedRoute />}>
        <Route element={<DashboardLayout />}>
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/deposit" element={<MoneyPage action="deposit" />} />
          <Route path="/withdraw" element={<MoneyPage action="withdraw" />} />
          <Route path="/transfer" element={<TransferPage />} />
          <Route path="/transactions" element={<TransactionsPage />} />
          <Route path="/contacts" element={<ContactsPage />} />
          <Route path="/architecture" element={<ArchitecturePage />} />
        </Route>
      </Route>
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  )
}
