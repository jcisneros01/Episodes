import type { AuthTokens } from '../types/auth'

const STORAGE_KEY = 'episodes.auth.tokens'
export const AUTH_STORAGE_EVENT = 'episodes:auth-change'

function dispatchAuthChange() {
  window.dispatchEvent(new Event(AUTH_STORAGE_EVENT))
}

export function getStoredTokens(): AuthTokens | null {
  const rawValue = window.localStorage.getItem(STORAGE_KEY)

  if (!rawValue) {
    return null
  }

  try {
    return JSON.parse(rawValue) as AuthTokens
  } catch {
    window.localStorage.removeItem(STORAGE_KEY)
    return null
  }
}

export function setStoredTokens(tokens: AuthTokens) {
  window.localStorage.setItem(STORAGE_KEY, JSON.stringify(tokens))
  dispatchAuthChange()
}

export function clearStoredTokens() {
  window.localStorage.removeItem(STORAGE_KEY)
  dispatchAuthChange()
}
