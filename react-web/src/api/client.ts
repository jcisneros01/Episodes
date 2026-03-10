import { clearStoredTokens, getStoredTokens, setStoredTokens } from './authStorage'
import type { AuthTokens } from '../types/auth'

interface ApiRequestInit extends RequestInit {
  skipAuth?: boolean
}

let refreshRequest: Promise<AuthTokens | null> | null = null

function buildHeaders(headers: HeadersInit | undefined, tokens: AuthTokens | null) {
  const requestHeaders = new Headers(headers)

  if (!requestHeaders.has('Accept')) {
    requestHeaders.set('Accept', 'application/json')
  }

  if (tokens?.accessToken) {
    requestHeaders.set('Authorization', `${tokens.tokenType} ${tokens.accessToken}`)
  }

  return requestHeaders
}

function firstError(errors: unknown): string | null {
  if (!errors || typeof errors !== 'object') {
    return null
  }

  for (const value of Object.values(errors)) {
    if (Array.isArray(value)) {
      const firstMessage = value.find((item): item is string => typeof item === 'string')

      if (firstMessage) {
        return firstMessage
      }
    }
  }

  return null
}

export async function readErrorMessage(response: Response, fallback: string) {
  const contentType = response.headers.get('content-type') ?? ''

  if (!contentType.includes('application/json')) {
    const text = await response.text()
    return text || fallback
  }

  try {
    const payload = (await response.json()) as {
      detail?: string
      title?: string
      errors?: unknown
    }

    return payload.detail ?? payload.title ?? firstError(payload.errors) ?? fallback
  } catch {
    return fallback
  }
}

async function refreshTokens() {
  const tokens = getStoredTokens()

  if (!tokens?.refreshToken) {
    clearStoredTokens()
    return null
  }

  if (!refreshRequest) {
    refreshRequest = (async () => {
      const response = await fetch('/api/auth/refresh', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json',
        },
        body: JSON.stringify({ refreshToken: tokens.refreshToken }),
      })

      if (!response.ok) {
        clearStoredTokens()
        return null
      }

      const refreshedTokens = (await response.json()) as AuthTokens
      setStoredTokens(refreshedTokens)
      return refreshedTokens
    })().finally(() => {
      refreshRequest = null
    })
  }

  return refreshRequest
}

export async function apiFetch(input: string, init: ApiRequestInit = {}) {
  const { skipAuth = false, headers, ...rest } = init
  const tokens = skipAuth ? null : getStoredTokens()

  const doRequest = (requestTokens: AuthTokens | null) =>
    fetch(input, {
      ...rest,
      headers: buildHeaders(headers, requestTokens),
    })

  const response = await doRequest(tokens)

  if (skipAuth || response.status !== 401 || !tokens?.refreshToken) {
    return response
  }

  const refreshedTokens = await refreshTokens()

  if (!refreshedTokens) {
    return response
  }

  return doRequest(refreshedTokens)
}

export async function fetchJson<T>(input: string, init: ApiRequestInit = {}) {
  const response = await apiFetch(input, init)

  if (!response.ok) {
    throw new Error(
      await readErrorMessage(
        response,
        `Request failed: ${response.status} ${response.statusText}`,
      ),
    )
  }

  return response.json() as Promise<T>
}
