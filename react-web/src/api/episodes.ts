import { apiFetch } from './client'

export async function markEpisodeWatched(episodeId: number): Promise<void> {
  const response = await apiFetch(`/api/episodes/${episodeId}/watched`, { method: 'POST' })
  if (!response.ok) {
    throw new Error(`Failed to mark episode as watched`)
  }
}

export async function markEpisodeUnwatched(episodeId: number): Promise<void> {
  const response = await apiFetch(`/api/episodes/${episodeId}/watched`, { method: 'DELETE' })
  if (!response.ok) {
    throw new Error(`Failed to mark episode as unwatched`)
  }
}
