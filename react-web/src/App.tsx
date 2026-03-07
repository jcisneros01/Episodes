import { useState } from 'react'
import { useSearch } from './hooks/useSearch'
import { SearchBar } from './components/SearchBar'
import { ShowGrid } from './components/ShowGrid'
import { ShowDetail } from './components/ShowDetail'
import { Pagination } from './components/Pagination'
import { AuthScreen } from './components/AuthScreen'
import { useAuth } from './context/AuthContext'

type AppView = { view: 'search' } | { view: 'detail'; showId: number }

function LoadingScreen() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-stone-950 text-stone-100">
      <div className="rounded-full border border-white/10 bg-white/5 px-5 py-3 text-sm tracking-[0.35em] text-stone-400">
        RESTORING SESSION
      </div>
    </div>
  )
}

function AuthenticatedApp({
  email,
  emailConfirmed,
  onLogout,
}: {
  email: string
  emailConfirmed: boolean
  onLogout: () => void
}) {
  const { query, page, data, loading, error, handleQueryChange, handlePageChange } = useSearch()
  const [appView, setAppView] = useState<AppView>({ view: 'search' })

  return (
    <div className="min-h-screen bg-gray-950 text-gray-100">
      <header className="border-b border-gray-800 bg-gray-900 px-6 py-4">
        <div className="mx-auto flex max-w-7xl flex-col gap-4 lg:flex-row lg:items-center">
          <h1
            className="cursor-pointer text-xl font-bold tracking-tight text-indigo-400"
            onClick={() => setAppView({ view: 'search' })}
          >
            Episodes
          </h1>

          {appView.view === 'search' && (
            <div className="min-w-0 flex-1">
              <SearchBar value={query} onChange={handleQueryChange} />
            </div>
          )}

          <div className="flex flex-wrap items-center gap-3 lg:ml-auto">
            <div className="rounded-full border border-gray-700 bg-gray-800 px-4 py-2 text-sm text-gray-300">
              {email}
              <span className="ml-2 text-xs uppercase tracking-[0.25em] text-gray-500">
                {emailConfirmed ? 'verified' : 'unverified'}
              </span>
            </div>
            <button
              onClick={onLogout}
              className="rounded-full border border-gray-700 px-4 py-2 text-sm font-medium text-gray-200 transition hover:border-indigo-500/40 hover:text-indigo-300"
            >
              Sign out
            </button>
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-7xl space-y-6 px-6 py-8">
        {appView.view === 'search' ? (
          <>
            <ShowGrid
              data={data}
              loading={loading}
              error={error}
              query={query}
              onShowClick={(showId) => setAppView({ view: 'detail', showId })}
            />

            {data && (
              <Pagination
                page={page}
                totalPages={data.total_pages}
                totalResults={data.total_results}
                onPageChange={handlePageChange}
              />
            )}
          </>
        ) : (
          <ShowDetail
            showId={appView.showId}
            onBack={() => setAppView({ view: 'search' })}
          />
        )}
      </main>
    </div>
  )
}

export function App() {
  const { status, user, error, isSubmitting, login, register, logout } = useAuth()

  if (status === 'loading') {
    return <LoadingScreen />
  }

  if (!user) {
    return (
      <AuthScreen
        error={error}
        submitting={isSubmitting}
        onLogin={login}
        onRegister={register}
      />
    )
  }

  return (
    <AuthenticatedApp
      email={user.email}
      emailConfirmed={user.emailConfirmed}
      onLogout={logout}
    />
  )
}
