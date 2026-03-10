import { apiFetch, fetchJson, readErrorMessage } from './client'
import { clearStoredTokens, setStoredTokens } from './authStorage'
import type { AuthTokens, AuthUser, CurrentUserResponse } from '../types/auth'

function mapCurrentUser(response: CurrentUserResponse): AuthUser {
  return {
    id: response.id,
    email: response.email,
    emailConfirmed: response.email_confirmed,
    createdAt: response.created_at,
  }
}

export async function registerUser(email: string, password: string) {
  const response = await apiFetch('/api/auth/register', {
    method: 'POST',
    skipAuth: true,
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password }),
  })

  if (!response.ok) {
    throw new Error(await readErrorMessage(response, 'Unable to create your account.'))
  }
}

export async function getCurrentUser() {
  const response = await fetchJson<CurrentUserResponse>('/api/me')
  return mapCurrentUser(response)
}

export async function loginUser(email: string, password: string) {
  const tokens = await fetchJson<AuthTokens>('/api/auth/login?useCookies=false', {
    method: 'POST',
    skipAuth: true,
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password }),
  })

  setStoredTokens(tokens)

  try {
    return await getCurrentUser()
  } catch (error) {
    clearStoredTokens()
    throw error
  }
}

export async function registerAndLoginUser(email: string, password: string) {
  await registerUser(email, password)
  return loginUser(email, password)
}

export function logoutUser() {
  clearStoredTokens()
}
