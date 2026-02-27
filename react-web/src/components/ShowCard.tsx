import type { TvSearchResult } from '../types/shows'

const TMDB_IMAGE_BASE = 'https://image.tmdb.org/t/p/w300'

interface ShowCardProps {
  show: TvSearchResult
}

export function ShowCard({ show }: ShowCardProps) {
  return (
    <article className="flex flex-col overflow-hidden rounded-xl bg-gray-800 shadow-md transition hover:shadow-indigo-500/20 hover:ring-1 hover:ring-indigo-500/40">
      <div className="aspect-[2/3] w-full bg-gray-700">
        {show.poster_path ? (
          <img
            src={`${TMDB_IMAGE_BASE}${show.poster_path}`}
            alt={`${show.name} poster`}
            className="h-full w-full object-cover"
            loading="lazy"
          />
        ) : (
          <div className="flex h-full items-center justify-center">
            <svg
              className="h-12 w-12 text-gray-600"
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
      <div className="flex flex-1 flex-col gap-1.5 p-3">
        <h3 className="line-clamp-2 text-sm font-semibold text-gray-100">{show.name}</h3>
        {show.overview && (
          <p className="line-clamp-3 text-xs leading-relaxed text-gray-400">{show.overview}</p>
        )}
      </div>
    </article>
  )
}
