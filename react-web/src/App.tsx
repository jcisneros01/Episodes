import { useState } from 'react'
import { useSearch } from './hooks/useSearch'
import { useWatchlist } from './hooks/useWatchlist'
import { SearchBar } from './components/SearchBar'
import { ShowGrid } from './components/ShowGrid'
import { ShowDetail } from './components/ShowDetail'
import { WatchlistPage } from './components/WatchlistPage'
import { Pagination } from './components/Pagination'
import { AuthScreen } from './components/AuthScreen'
import { useAuth } from './context/AuthContext'

type AppView =
  | { view: 'search' }
  | { view: 'watchlist' }
  | { view: 'detail'; showId: number; idType: 'internal' | 'external' }

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
  const watchlist = useWatchlist()
  const [appView, setAppView] = useState<AppView>({ view: 'search' })

  return (
    <div className="min-h-screen bg-gray-950 text-gray-100">
      <header className="border-b border-gray-800 bg-gray-900 px-6 py-4">
        <div className="mx-auto flex max-w-7xl flex-col gap-4 lg:flex-row lg:items-center">
          <div className="flex items-center gap-4">
            <h1
              className="cursor-pointer text-xl font-bold tracking-tight text-indigo-400"
              onClick={() => setAppView({ view: 'search' })}
            >
              Episodes
            </h1>
            <button
              onClick={() => setAppView({ view: 'watchlist' })}
              className={`rounded-full px-3 py-1.5 text-sm font-medium transition ${
                appView.view === 'watchlist'
                  ? 'bg-indigo-500/20 text-indigo-300'
                  : 'text-gray-400 hover:text-indigo-300'
              }`}
            >
              Watchlist
            </button>
          </div>

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
        {appView.view === 'search' && (
          <>
            <ShowGrid
              data={data}
              loading={loading}
              error={error}
              query={query}
              onShowClick={(showId) => setAppView({ view: 'detail', showId, idType: 'external' })}
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
        )}

        {appView.view === 'watchlist' && (
          <WatchlistPage
            items={watchlist.items}
            loading={watchlist.loading}
            error={watchlist.error}
            onShowClick={(showId) => setAppView({ view: 'detail', showId, idType: 'internal' })}
            onRemove={watchlist.remove}
          />
        )}

        {appView.view === 'detail' && (
          <ShowDetail
            showId={appView.showId}
            idType={appView.idType}
            onBack={() => setAppView({ view: 'search' })}
            isOnWatchlist={watchlist.isOnWatchlist}
            onAddToWatchlist={watchlist.add}
            onRemoveFromWatchlist={watchlist.remove}
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
