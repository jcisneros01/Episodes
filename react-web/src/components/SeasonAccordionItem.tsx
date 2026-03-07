import { useState } from 'react'
import { getSeasonEpisodes } from '../api/shows'
import type { Episode, TvSeasonSummary } from '../types/shows'

interface SeasonAccordionItemProps {
  tvShowId: number
  season: TvSeasonSummary
}

function EpisodeList({ episodes }: { episodes: Episode[] }) {
  if (episodes.length === 0) {
    return <p className="text-sm text-gray-400">No episodes returned for this season.</p>
  }

  return (
    <div className="space-y-3">
      {episodes.map((episode) => (
        <div key={episode.id} className="rounded-lg bg-gray-800/80 p-4">
          <div className="flex items-baseline justify-between gap-3">
            <h4 className="font-medium text-gray-100">
              Episode {episode.episode_number}: {episode.name}
            </h4>
            {episode.air_date && <span className="text-xs text-gray-500">{episode.air_date}</span>}
          </div>
          {episode.overview && <p className="mt-2 text-sm leading-6 text-gray-400">{episode.overview}</p>}
        </div>
      ))}
    </div>
  )
}

export function SeasonAccordionItem({ tvShowId, season }: SeasonAccordionItemProps) {
  const [isOpen, setIsOpen] = useState(false)
  const [loading, setLoading] = useState(false)
  const [episodes, setEpisodes] = useState<Episode[] | null>(null)
  const [error, setError] = useState<string | null>(null)

  async function handleToggle() {
    const nextIsOpen = !isOpen
    setIsOpen(nextIsOpen)

    if (!nextIsOpen || episodes || loading) {
      return
    }

    setLoading(true)
    setError(null)

    try {
      const seasonResponse = await getSeasonEpisodes(tvShowId, season.season_number)
      setEpisodes(seasonResponse.episodes)
    } catch (seasonError) {
      setError(seasonError instanceof Error ? seasonError.message : 'Failed to load season.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <section className="overflow-hidden rounded-xl border border-gray-800 bg-gray-900">
      <button
        type="button"
        onClick={handleToggle}
        className="flex w-full items-center justify-between gap-4 px-5 py-4 text-left transition hover:bg-gray-800/70"
      >
        <div>
          <h4 className="font-medium text-gray-100">
            Season {season.season_number}
            {season.name ? `: ${season.name}` : ''}
          </h4>
          <p className="mt-1 text-sm text-gray-400">
            {season.episode_count} episode{season.episode_count === 1 ? '' : 's'}
            {season.air_date ? ` • ${season.air_date}` : ''}
          </p>
        </div>

        <svg
          className={`h-5 w-5 shrink-0 text-gray-400 transition ${isOpen ? 'rotate-180' : ''}`}
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          strokeWidth={2}
        >
          <path strokeLinecap="round" strokeLinejoin="round" d="m19 9-7 7-7-7" />
        </svg>
      </button>

      {isOpen && (
        <div className="space-y-4 border-t border-gray-800 px-5 py-4">
          {season.overview && <p className="text-sm leading-6 text-gray-400">{season.overview}</p>}
          {loading && <p className="text-sm text-gray-400">Loading episodes...</p>}
          {error && <p className="text-sm text-red-300">{error}</p>}
          {episodes && <EpisodeList episodes={episodes} />}
        </div>
      )}
    </section>
  )
}
