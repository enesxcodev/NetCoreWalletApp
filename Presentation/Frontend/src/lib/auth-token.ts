const TOKEN_KEY = 'wallet.auth.token'

/** Oturum belirtecini sekme kapanana kadar sessionStorage içinde tutar. */
export const authToken = {
  get(): string | null {
    return sessionStorage.getItem(TOKEN_KEY)
  },
  set(token: string): void {
    sessionStorage.setItem(TOKEN_KEY, token)
  },
  clear(): void {
    sessionStorage.removeItem(TOKEN_KEY)
  },
}
