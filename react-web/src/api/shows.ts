import type { TvShowSearchResponse, TvShowResponse, TvSeasonResponse } from '../types/shows'

export async function searchShows(
  query: string,
  page = 1,
  signal?: AbortSignal,
): Promise<TvShowSearchResponse> {
  const params = new URLSearchParams({ query, page: String(page) })
  const response = await fetch(`/api/shows/search?${params}`, { signal })

  if (!response.ok) {
    throw new Error(`Search failed: ${response.status} ${response.statusText}`)
  }

  return response.json() as Promise<TvShowSearchResponse>
}

export async function getShow(
  tvShowId: number,
  signal?: AbortSignal,
): Promise<TvShowResponse> {
  const response = await fetch(`/api/shows/${tvShowId}`, { signal })

  if (!response.ok) {
    throw new Error(`Failed to load show: ${response.status} ${response.statusText}`)
  }

  return response.json() as Promise<TvShowResponse>
}

export async function getSeasonEpisodes(
  tvShowId: number,
  seasonNumber: number,
  signal?: AbortSignal,
): Promise<TvSeasonResponse> {
  const response = await fetch(`/api/shows/${tvShowId}/season/${seasonNumber}`, { signal })

  if (!response.ok) {
    throw new Error(`Failed to load season: ${response.status} ${response.statusText}`)
  }

  return response.json() as Promise<TvSeasonResponse>
}
