import { createContext, useContext, useEffect, useState, type ReactNode } from 'react'
import { AUTH_STORAGE_EVENT, getStoredTokens } from '../api/authStorage'
import { getCurrentUser, loginUser, logoutUser, registerAndLoginUser } from '../api/auth'
import type { AuthUser } from '../types/auth'

type AuthStatus = 'loading' | 'anonymous' | 'authenticated'

interface AuthContextValue {
  status: AuthStatus
  user: AuthUser | null
  error: string | null
  isSubmitting: boolean
  login: (email: string, password: string) => Promise<void>
  register: (email: string, password: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

function toErrorMessage(error: unknown, fallback: string) {
  return error instanceof Error ? error.message : fallback
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [status, setStatus] = useState<AuthStatus>('loading')
  const [user, setUser] = useState<AuthUser | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    let cancelled = false

    async function restoreSession() {
      const tokens = getStoredTokens()

      if (!tokens) {
        if (!cancelled) {
          setStatus('anonymous')
          setUser(null)
        }

        return
      }

      try {
        const currentUser = await getCurrentUser()

        if (!cancelled) {
          setUser(currentUser)
          setStatus('authenticated')
          setError(null)
        }
      } catch (restoreError) {
        if (!cancelled) {
          logoutUser()
          setUser(null)
          setStatus('anonymous')
          setError(toErrorMessage(restoreError, 'Your session expired. Sign in again.'))
        }
      }
    }

    void restoreSession()

    const handleSessionChange = () => {
      if (!getStoredTokens()) {
        setUser(null)
        setStatus('anonymous')
      }
    }

    window.addEventListener(AUTH_STORAGE_EVENT, handleSessionChange)
    window.addEventListener('storage', handleSessionChange)

    return () => {
      cancelled = true
      window.removeEventListener(AUTH_STORAGE_EVENT, handleSessionChange)
      window.removeEventListener('storage', handleSessionChange)
    }
  }, [])

  async function login(email: string, password: string) {
    setIsSubmitting(true)
    setError(null)

    try {
      const currentUser = await loginUser(email, password)
      setUser(currentUser)
      setStatus('authenticated')
    } catch (loginError) {
      setUser(null)
      setStatus('anonymous')
      setError(toErrorMessage(loginError, 'Unable to sign in right now.'))
      throw loginError
    } finally {
      setIsSubmitting(false)
    }
  }

  async function register(email: string, password: string) {
    setIsSubmitting(true)
    setError(null)

    try {
      const currentUser = await registerAndLoginUser(email, password)
      setUser(currentUser)
      setStatus('authenticated')
    } catch (registrationError) {
      setUser(null)
      setStatus('anonymous')
      setError(toErrorMessage(registrationError, 'Unable to create your account right now.'))
      throw registrationError
    } finally {
      setIsSubmitting(false)
    }
  }

  function logout() {
    logoutUser()
    setUser(null)
    setStatus('anonymous')
    setError(null)
  }

  return (
    <AuthContext.Provider
      value={{
        status,
        user,
        error,
        isSubmitting,
        login,
        register,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }

  return context
}
