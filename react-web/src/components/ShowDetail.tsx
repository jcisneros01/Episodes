import { useShowDetail } from '../hooks/useShowDetail'
import { SeasonAccordionItem } from './SeasonAccordionItem'

const TMDB_IMAGE_BASE = 'https://image.tmdb.org/t/p/w500'

interface ShowDetailProps {
  showId: number
  onBack: () => void
}

function DetailSkeleton() {
  return (
    <div className="space-y-8">
      <div className="flex flex-col gap-8 md:flex-row">
        <div className="aspect-[2/3] w-full max-w-xs animate-pulse rounded-xl bg-gray-700" />
        <div className="flex-1 space-y-4">
          <div className="h-8 w-2/3 animate-pulse rounded bg-gray-700" />
          <div className="h-4 w-1/4 animate-pulse rounded bg-gray-700" />
          <div className="flex gap-2">
            <div className="h-6 w-16 animate-pulse rounded-full bg-gray-700" />
            <div className="h-6 w-20 animate-pulse rounded-full bg-gray-700" />
            <div className="h-6 w-14 animate-pulse rounded-full bg-gray-700" />
          </div>
          <div className="space-y-2">
            <div className="h-4 animate-pulse rounded bg-gray-700" />
            <div className="h-4 animate-pulse rounded bg-gray-700" />
            <div className="h-4 w-3/4 animate-pulse rounded bg-gray-700" />
          </div>
        </div>
      </div>
    </div>
  )
}

export function ShowDetail({ showId, onBack }: ShowDetailProps) {
  const { show, loading, error } = useShowDetail(showId)

  return (
    <div className="space-y-6">
      <button
        onClick={onBack}
        className="flex items-center gap-1.5 text-sm text-gray-400 transition hover:text-indigo-400"
      >
        <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
          <path strokeLinecap="round" strokeLinejoin="round" d="M15 19l-7-7 7-7" />
        </svg>
        Back to search
      </button>

      {loading && <DetailSkeleton />}

      {error && (
        <div className="flex flex-col items-center gap-3 py-16 text-center">
          <svg
            className="h-12 w-12 text-red-400"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            strokeWidth={1.5}
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z"
            />
          </svg>
          <p className="text-gray-300">{error}</p>
        </div>
      )}

      {show && (
        <div className="space-y-8">
          {/* Hero section */}
          <div className="flex flex-col gap-8 md:flex-row">
            <div className="w-full max-w-xs shrink-0">
              {show.poster_path ? (
                <img
                  src={`${TMDB_IMAGE_BASE}${show.poster_path}`}
                  alt={`${show.name} poster`}
                  className="w-full rounded-xl shadow-lg"
                />
              ) : (
                <div className="flex aspect-[2/3] items-center justify-center rounded-xl bg-gray-700">
                  <svg
                    className="h-16 w-16 text-gray-600"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                    strokeWidth={1}
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M3.375 19.5h17.25m-17.25 0a1.125 1.125 0 0 1-1.125-1.125M3.375 19.5h1.5C5.496 19.5 6 18.996 6 18.375m-3.75.125v-5.625M5.25 4.5h13.5A2.25 2.25 0 0 1 21 6.75v8.25A2.25 2.25 0 0 1 18.75 17.25H5.25A2.25 2.25 0 0 1 3 15V6.75A2.25 2.25 0 0 1 5.25 4.5Z"
                    />
                  </svg>
                </div>
              )}
            </div>

            <div className="flex-1 space-y-4">
              <h2 className="text-3xl font-bold text-gray-100">{show.name}</h2>

              <div className="flex flex-wrap items-center gap-x-4 gap-y-1 text-sm text-gray-400">
                {show.first_air_date && <span>{show.first_air_date}</span>}
                <span className="rounded bg-gray-700 px-2 py-0.5 text-xs font-medium text-gray-300">
                  {show.status}
                </span>
                <span>
                  {show.number_of_seasons} season{show.number_of_seasons !== 1 ? 's' : ''}
                  {' \u00B7 '}
                  {show.number_of_episodes} episode{show.number_of_episodes !== 1 ? 's' : ''}
                </span>
              </div>

              {show.genres.length > 0 && (
                <div className="flex flex-wrap gap-2">
                  {show.genres.map((genre) => (
                    <span
                      key={genre}
                      className="rounded-full bg-indigo-500/20 px-3 py-1 text-xs font-medium text-indigo-400"
                    >
                      {genre}
                    </span>
                  ))}
                </div>
              )}

              {show.networks.length > 0 && (
                <p className="text-sm text-gray-400">
                  <span className="text-gray-500">Networks:</span> {show.networks.join(', ')}
                </p>
              )}

              {show.overview && (
                <p className="leading-relaxed text-gray-300">{show.overview}</p>
              )}
            </div>
          </div>

          {/* Seasons */}
          {show.seasons.length > 0 && (
            <div className="space-y-4">
              <h3 className="text-xl font-semibold text-gray-100">Seasons</h3>
              <div className="space-y-3">
                {show.seasons.map((season) => (
                  <SeasonAccordionItem key={season.id} tvShowId={show.id} season={season} />
                ))}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  )
}
