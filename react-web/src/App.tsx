import { useState } from 'react'
import { useSearch } from './hooks/useSearch'
import { SearchBar } from './components/SearchBar'
import { ShowGrid } from './components/ShowGrid'
import { ShowDetail } from './components/ShowDetail'
import { Pagination } from './components/Pagination'

type AppView = { view: 'search' } | { view: 'detail'; showId: number }

export function App() {
  const { query, page, data, loading, error, handleQueryChange, handlePageChange } = useSearch()
  const [appView, setAppView] = useState<AppView>({ view: 'search' })

  return (
    <div className="min-h-screen bg-gray-950 text-gray-100">
      <header className="border-b border-gray-800 bg-gray-900 px-6 py-4">
        <div className="mx-auto flex max-w-7xl items-center gap-6">
          <h1
            className="cursor-pointer text-xl font-bold tracking-tight text-indigo-400"
            onClick={() => setAppView({ view: 'search' })}
          >
            Episodes
          </h1>
          {appView.view === 'search' && (
            <SearchBar value={query} onChange={handleQueryChange} />
          )}
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
