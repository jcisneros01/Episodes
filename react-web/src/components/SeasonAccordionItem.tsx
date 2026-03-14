import { useState } from 'react'
import { getSeasonEpisodes } from '../api/shows'
import { markEpisodeWatched, markEpisodeUnwatched } from '../api/episodes'
import type { Episode, TvSeasonSummary } from '../types/shows'

interface SeasonAccordionItemProps {
  tvShowId: number
  season: TvSeasonSummary
}

function EpisodeItem({ episode, onToggle }: { episode: Episode; onToggle: (id: number) => void }) {
  const [toggling, setToggling] = useState(false)

  async function handleToggle() {
    setToggling(true)
    try {
      if (episode.is_watched) {
        await markEpisodeUnwatched(episode.id)
      } else {
        await markEpisodeWatched(episode.id)
      }
      onToggle(episode.id)
    } catch {
      // silently fail — state stays unchanged
    } finally {
      setToggling(false)
    }
  }

  return (
    <div key={episode.id} className="rounded-lg bg-gray-800/80 p-4">
      <div className="flex items-center justify-between gap-3">
        <div className="min-w-0 flex-1">
          <div className="flex items-baseline gap-3">
            <h4 className="font-medium text-gray-100">
              Episode {episode.episode_number}: {episode.name}
            </h4>
            {episode.air_date && <span className="text-xs text-gray-500">{episode.air_date}</span>}
          </div>
          {episode.overview && <p className="mt-2 text-sm leading-6 text-gray-400">{episode.overview}</p>}
        </div>
        <button
          type="button"
          onClick={handleToggle}
          disabled={toggling}
          className={`shrink-0 rounded-full px-3 py-1.5 text-xs font-medium transition ${
            episode.is_watched
              ? 'bg-indigo-500/20 text-indigo-300 hover:bg-indigo-500/30'
              : 'bg-gray-700 text-gray-400 hover:bg-gray-600 hover:text-gray-200'
          } disabled:opacity-50`}
        >
          {toggling ? '...' : episode.is_watched ? 'Watched' : 'Mark watched'}
        </button>
      </div>
    </div>
  )
}

function EpisodeList({
  episodes,
  onToggle,
}: {
  episodes: Episode[]
  onToggle: (id: number) => void
}) {
  if (episodes.length === 0) {
    return <p className="text-sm text-gray-400">No episodes returned for this season.</p>
  }

  return (
    <div className="space-y-3">
      {episodes.map((episode) => (
        <EpisodeItem key={episode.id} episode={episode} onToggle={onToggle} />
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

  function handleEpisodeToggle(episodeId: number) {
    setEpisodes((prev) =>
      prev
        ? prev.map((ep) => (ep.id === episodeId ? { ...ep, is_watched: !ep.is_watched } : ep))
        : prev,
    )
  }

  const watchedCount = episodes?.filter((ep) => ep.is_watched).length ?? 0
  const totalCount = episodes?.length ?? 0

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
            {episodes && totalCount > 0 && (
              <span className="ml-2 text-indigo-400">
                {watchedCount}/{totalCount} watched
              </span>
            )}
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
          {episodes && <EpisodeList episodes={episodes} onToggle={handleEpisodeToggle} />}
        </div>
      )}
    </section>
  )
}
