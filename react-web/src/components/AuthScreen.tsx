import { useState, type FormEvent } from 'react'

type AuthMode = 'sign-in' | 'register'

interface AuthScreenProps {
  error: string | null
  submitting: boolean
  onLogin: (email: string, password: string) => Promise<void>
  onRegister: (email: string, password: string) => Promise<void>
}

function toLocalError(mode: AuthMode, email: string, password: string, confirmPassword: string) {
  if (!email.trim()) {
    return 'Email is required.'
  }

  if (!password) {
    return 'Password is required.'
  }

  if (mode === 'register' && password.length < 8) {
    return 'Password must be at least 8 characters.'
  }

  if (mode === 'register' && !/\d/.test(password)) {
    return 'Password must include at least one number.'
  }

  if (mode === 'register' && !/[a-z]/.test(password)) {
    return 'Password must include at least one lowercase letter.'
  }

  if (mode === 'register' && password !== confirmPassword) {
    return 'Passwords do not match.'
  }

  return null
}

export function AuthScreen({
  error,
  submitting,
  onLogin,
  onRegister,
}: AuthScreenProps) {
  const [mode, setMode] = useState<AuthMode>('sign-in')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [localError, setLocalError] = useState<string | null>(null)

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    const validationError = toLocalError(mode, email, password, confirmPassword)

    if (validationError) {
      setLocalError(validationError)
      return
    }

    setLocalError(null)

    try {
      if (mode === 'register') {
        await onRegister(email.trim(), password)
        return
      }

      await onLogin(email.trim(), password)
    } catch {
      // AuthContext exposes the server error, so there is nothing to do here.
    }
  }

  const activeError = localError ?? error

  return (
    <div className="min-h-screen bg-gray-950 text-gray-100">
      <header className="border-b border-gray-800 bg-gray-900 px-6 py-4">
        <div className="mx-auto max-w-7xl">
          <h1 className="text-xl font-bold tracking-tight text-indigo-400">Episodes</h1>
        </div>
      </header>

      <main className="mx-auto flex min-h-[calc(100vh-73px)] max-w-7xl items-center justify-center px-6 py-10">
        <section className="w-full max-w-md rounded-2xl border border-gray-800 bg-gray-900 p-8 shadow-xl shadow-black/20">
          <div className="mb-8 flex rounded-full border border-gray-700 bg-gray-800 p-1">
              <button
                type="button"
                onClick={() => setMode('sign-in')}
                className={`flex-1 rounded-full px-4 py-2 text-sm font-medium transition ${
                  mode === 'sign-in'
                    ? 'bg-indigo-500 text-white'
                    : 'text-gray-400 hover:text-gray-100'
                }`}
              >
                Sign in
              </button>
              <button
                type="button"
                onClick={() => setMode('register')}
                className={`flex-1 rounded-full px-4 py-2 text-sm font-medium transition ${
                  mode === 'register'
                    ? 'bg-indigo-500 text-white'
                    : 'text-gray-400 hover:text-gray-100'
                }`}
              >
                Create account
              </button>
          </div>

          <div className="mb-6 space-y-2">
            <h2 className="text-2xl font-semibold text-gray-100">
              {mode === 'register' ? 'Create your account' : 'Sign in to continue'}
            </h2>
            <p className="text-sm leading-6 text-gray-400">
              {mode === 'register'
                ? 'Create an account to access your Episodes workspace.'
                : 'Use your Episodes account to continue browsing shows.'}
            </p>
          </div>

          <form className="space-y-5" onSubmit={handleSubmit}>
            <label className="block space-y-2">
              <span className="text-sm font-medium text-gray-300">Email</span>
              <input
                type="email"
                autoComplete="email"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                className="w-full rounded-xl border border-gray-700 bg-gray-800 px-4 py-3 text-gray-100 outline-none transition placeholder:text-gray-500 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500/40"
                placeholder="you@example.com"
              />
            </label>

            <label className="block space-y-2">
              <span className="text-sm font-medium text-gray-300">Password</span>
              <input
                type="password"
                autoComplete={mode === 'register' ? 'new-password' : 'current-password'}
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                className="w-full rounded-xl border border-gray-700 bg-gray-800 px-4 py-3 text-gray-100 outline-none transition placeholder:text-gray-500 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500/40"
                placeholder="At least 8 characters"
              />
            </label>

            {mode === 'register' && (
              <label className="block space-y-2">
                <span className="text-sm font-medium text-gray-300">Confirm password</span>
                <input
                  type="password"
                  autoComplete="new-password"
                  value={confirmPassword}
                  onChange={(event) => setConfirmPassword(event.target.value)}
                  className="w-full rounded-xl border border-gray-700 bg-gray-800 px-4 py-3 text-gray-100 outline-none transition placeholder:text-gray-500 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500/40"
                  placeholder="Repeat your password"
                />
              </label>
            )}

            {activeError && (
              <div className="rounded-xl border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-200">
                {activeError}
              </div>
            )}

            <button
              type="submit"
              disabled={submitting}
              className="w-full rounded-xl bg-indigo-500 px-4 py-3 font-semibold text-white transition hover:bg-indigo-400 disabled:cursor-not-allowed disabled:opacity-70"
            >
              {submitting
                ? 'Connecting...'
                : mode === 'register'
                  ? 'Create account'
                  : 'Sign in'}
            </button>
          </form>

          <p className="mt-6 text-sm leading-6 text-gray-500">
            Password rules: minimum 8 characters, one digit, one lowercase letter.
          </p>
        </section>
      </main>
    </div>
  )
}
