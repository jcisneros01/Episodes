import type { WatchlistItem } from '../types/shows'

const TMDB_IMAGE_BASE = 'https://image.tmdb.org/t/p/w300'

interface WatchlistPageProps {
  items: WatchlistItem[]
  loading: boolean
  error: string | null
  onShowClick: (showId: number) => void
  onRemove: (showId: number) => void
}

function WatchlistSkeleton() {
  return (
    <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6">
      {Array.from({ length: 6 }).map((_, i) => (
        <div key={i} className="overflow-hidden rounded-xl bg-gray-800">
          <div className="aspect-[2/3] w-full animate-pulse bg-gray-700" />
          <div className="space-y-2 p-3">
            <div className="h-4 w-3/4 animate-pulse rounded bg-gray-700" />
            <div className="h-3 w-1/2 animate-pulse rounded bg-gray-700" />
          </div>
        </div>
      ))}
    </div>
  )
}

export function WatchlistPage({ items, loading, error, onShowClick, onRemove }: WatchlistPageProps) {
  if (loading) {
    return (
      <div className="space-y-6">
        <h2 className="text-2xl font-bold text-gray-100">My Watchlist</h2>
        <WatchlistSkeleton />
      </div>
    )
  }

  if (error) {
    return (
      <div className="space-y-6">
        <h2 className="text-2xl font-bold text-gray-100">My Watchlist</h2>
        <div className="flex flex-col items-center gap-3 py-16 text-center">
          <svg className="h-12 w-12 text-red-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z" />
          </svg>
          <p className="text-gray-300">{error}</p>
        </div>
      </div>
    )
  }

  if (items.length === 0) {
    return (
      <div className="space-y-6">
        <h2 className="text-2xl font-bold text-gray-100">My Watchlist</h2>
        <div className="flex flex-col items-center gap-3 py-16 text-center">
          <svg className="h-12 w-12 text-gray-600" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M3.375 19.5h17.25m-17.25 0a1.125 1.125 0 0 1-1.125-1.125M3.375 19.5h1.5C5.496 19.5 6 18.996 6 18.375m-3.75.125v-5.625M5.25 4.5h13.5A2.25 2.25 0 0 1 21 6.75v8.25A2.25 2.25 0 0 1 18.75 17.25H5.25A2.25 2.25 0 0 1 3 15V6.75A2.25 2.25 0 0 1 5.25 4.5Z" />
          </svg>
          <p className="text-gray-400">Your watchlist is empty. Search for shows to add.</p>
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-bold text-gray-100">My Watchlist</h2>
      <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6">
        {items.map((item) => (
          <article
            key={item.show_id}
            className="group relative flex cursor-pointer flex-col overflow-hidden rounded-xl bg-gray-800 shadow-md transition hover:shadow-indigo-500/20 hover:ring-1 hover:ring-indigo-500/40"
            onClick={() => onShowClick(item.show_id)}
          >
            <div className="aspect-[2/3] w-full bg-gray-700">
              {item.poster_img_link ? (
                <img
                  src={`${TMDB_IMAGE_BASE}${item.poster_img_link}`}
                  alt={`${item.name} poster`}
                  className="h-full w-full object-cover"
                  loading="lazy"
                />
              ) : (
                <div className="flex h-full items-center justify-center">
                  <svg className="h-12 w-12 text-gray-600" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1}>
                    <path strokeLinecap="round" strokeLinejoin="round" d="M3.375 19.5h17.25m-17.25 0a1.125 1.125 0 0 1-1.125-1.125M3.375 19.5h1.5C5.496 19.5 6 18.996 6 18.375m-3.75.125v-5.625M5.25 4.5h13.5A2.25 2.25 0 0 1 21 6.75v8.25A2.25 2.25 0 0 1 18.75 17.25H5.25A2.25 2.25 0 0 1 3 15V6.75A2.25 2.25 0 0 1 5.25 4.5Z" />
                  </svg>
                </div>
              )}
            </div>
            <div className="flex flex-1 flex-col gap-1.5 p-3">
              <h3 className="line-clamp-2 text-sm font-semibold text-gray-100">{item.name}</h3>
            </div>
            <button
              onClick={(e) => {
                e.stopPropagation()
                onRemove(item.show_id)
              }}
              className="absolute right-2 top-2 rounded-full bg-gray-900/80 p-1.5 text-gray-400 opacity-0 transition hover:bg-red-500/80 hover:text-white group-hover:opacity-100"
              title="Remove from watchlist"
            >
              <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </article>
        ))}
      </div>
    </div>
  )
}
