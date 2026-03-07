import { useState, useRef } from 'react'
import { getSeasonEpisodes } from '../api/shows'
import type { TvSeasonSummary, Episode } from '../types/shows'

interface SeasonAccordionItemProps {
  tvShowId: number
  season: TvSeasonSummary
}

export function SeasonAccordionItem({ tvShowId, season }: SeasonAccordionItemProps) {
  const [expanded, setExpanded] = useState(false)
  const [episodes, setEpisodes] = useState<Episode[] | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const fetched = useRef(false)

  const handleToggle = () => {
    const willExpand = !expanded
    setExpanded(willExpand)

    if (willExpand && !fetched.current) {
      fetched.current = true
      setLoading(true)
      setError(null)

      getSeasonEpisodes(tvShowId, season.season_number)
        .then((data) => {
          setEpisodes(data.episodes)
          setLoading(false)
        })
        .catch((err) => {
          const message = err instanceof Error ? err.message : 'Failed to load episodes'
          setError(message)
          setLoading(false)
          fetched.current = false
        })
    }
  }

  return (
    <div className="overflow-hidden rounded-xl border border-gray-700 bg-gray-800">
      <button
        onClick={handleToggle}
        className="flex w-full items-center justify-between px-5 py-4 text-left transition hover:bg-gray-750 hover:bg-gray-700/50"
      >
        <div className="flex items-center gap-3">
          <span className="font-semibold text-gray-100">{season.name}</span>
          <span className="text-sm text-gray-400">
            {season.episode_count} episode{season.episode_count !== 1 ? 's' : ''}
          </span>
        </div>
        <svg
          className={`h-5 w-5 text-gray-400 transition-transform ${expanded ? 'rotate-180' : ''}`}
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          strokeWidth={2}
        >
          <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
        </svg>
      </button>

      {expanded && (
        <div className="border-t border-gray-700 px-5 py-4">
          {loading && (
            <div className="space-y-3">
              {Array.from({ length: 3 }).map((_, i) => (
                <div key={i} className="flex gap-3">
                  <div className="h-6 w-10 animate-pulse rounded bg-gray-700" />
                  <div className="flex-1 space-y-2">
                    <div className="h-4 w-1/3 animate-pulse rounded bg-gray-700" />
                    <div className="h-3 w-full animate-pulse rounded bg-gray-700" />
                  </div>
                </div>
              ))}
            </div>
          )}

          {error && (
            <p className="text-sm text-red-400">{error}</p>
          )}

          {episodes && (
            <div className="space-y-3">
              {episodes.map((episode) => (
                <div key={episode.episode_number} className="flex gap-3">
                  <span className="mt-0.5 flex h-6 w-10 shrink-0 items-center justify-center rounded bg-indigo-500/20 text-xs font-bold text-indigo-400">
                    E{String(episode.episode_number).padStart(2, '0')}
                  </span>
                  <div className="min-w-0 flex-1">
                    <div className="flex items-baseline gap-2">
                      <span className="font-medium text-gray-100">{episode.name}</span>
                      {episode.air_date && (
                        <span className="shrink-0 text-xs text-gray-500">{episode.air_date}</span>
                      )}
                    </div>
                    {episode.overview && (
                      <p className="mt-1 text-sm leading-relaxed text-gray-400">{episode.overview}</p>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  )
}
