import { useSearch } from './hooks/useSearch'
import { SearchBar } from './components/SearchBar'
import { ShowGrid } from './components/ShowGrid'
import { Pagination } from './components/Pagination'

export function App() {
  const { query, page, data, loading, error, handleQueryChange, handlePageChange } = useSearch()

  return (
    <div className="min-h-screen bg-gray-950 text-gray-100">
      <header className="border-b border-gray-800 bg-gray-900 px-6 py-4">
        <div className="mx-auto flex max-w-7xl items-center gap-6">
          <h1 className="text-xl font-bold tracking-tight text-indigo-400">Episodes</h1>
          <SearchBar value={query} onChange={handleQueryChange} />
        </div>
      </header>

      <main className="mx-auto max-w-7xl space-y-6 px-6 py-8">
        <ShowGrid data={data} loading={loading} error={error} query={query} />

        {data && (
          <Pagination
            page={page}
            totalPages={data.total_pages}
            totalResults={data.total_results}
            onPageChange={handlePageChange}
          />
        )}
      </main>
    </div>
  )
}
