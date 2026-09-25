import { expect, test } from '@playwright/test'

test('girişten transfer onayına gider ve idempotency anahtarı gönderir', async ({ page }) => {
  let idempotencyKey = ''
  await page.route('**/api/Auth/login', async (route) => route.fulfill({ json: { isSuccess: true, data: 'e2e-session-token' } }))
  await page.route('**/api/Wallet', async (route) => route.fulfill({ json: { isSuccess: true, data: { code: 'WLT-12345678', balance: 2500 } } }))
  await page.route('**/api/Wallet/transaction**', async (route) => route.fulfill({ json: { isSuccess: true, data: [] } }))
  await page.route('**/api/Wallet/transfer', async (route) => {
    idempotencyKey = route.request().headers()['idempotency-key'] ?? ''
    await route.fulfill({ json: { isSuccess: true, data: '08d6e623-e3cf-4a68-a72f-a982bd7775ac' } })
  })

  await page.goto('/login')
  await page.getByLabel('Kullanıcı adı').fill('demo-user')
  await page.getByRole('textbox', { name: 'Şifre' }).fill('secret123')
  await page.getByRole('button', { name: /Giriş yap/ }).click()
  await expect(page).toHaveURL(/dashboard/)
  await page.getByRole('link', { name: 'Transfer', exact: true }).click()
  await page.getByLabel('Alıcı cüzdan kodu').fill('WLT-87654321')
  await page.getByLabel(/Tutar/).fill('25')
  await page.getByRole('button', { name: /Özeti görüntüle/ }).click()
  await page.getByRole('button', { name: /Transferi onayla/ }).click()
  await expect(page.getByText(/08d6e623-e3cf-4a68-a72f-a982bd7775ac/)).toBeVisible()
  expect(idempotencyKey).toMatch(/^[0-9a-f-]{36}$/i)
})

test('mobil genişlikte giriş formu yatay taşma yapmaz', async ({ page }) => {
  await page.setViewportSize({ width: 390, height: 844 })
  await page.goto('/login')
  await expect(page.getByRole('heading', { name: 'Tekrar hoş geldiniz' })).toBeVisible()
  const widths = await page.evaluate(() => ({ viewport: document.documentElement.clientWidth, page: document.documentElement.scrollWidth }))
  expect(widths.page).toBeLessThanOrEqual(widths.viewport)
})
