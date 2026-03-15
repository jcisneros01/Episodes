import { apiFetch, fetchJson } from './client'
import type { TvShowSearchResponse, TvShowResponse, TvSeasonResponse, WatchlistItem } from '../types/shows'

export async function searchShows(
  query: string,
  page = 1,
  signal?: AbortSignal,
): Promise<TvShowSearchResponse> {
  const params = new URLSearchParams({ query, page: String(page) })
  return fetchJson<TvShowSearchResponse>(`/api/shows/search?${params}`, { signal })
}

export async function getShow(
  showId: number,
  signal?: AbortSignal,
): Promise<TvShowResponse> {
  return fetchJson<TvShowResponse>(`/api/shows/${showId}`, { signal })
}

export async function getShowByExternalId(
  externalId: number,
  signal?: AbortSignal,
): Promise<TvShowResponse> {
  return fetchJson<TvShowResponse>(`/api/shows/external/${externalId}`, { signal })
}

export async function getSeasonEpisodes(
  showId: number,
  seasonNumber: number,
  signal?: AbortSignal,
): Promise<TvSeasonResponse> {
  return fetchJson<TvSeasonResponse>(`/api/shows/${showId}/season/${seasonNumber}`, { signal })
}

export async function getWatchlist(signal?: AbortSignal): Promise<WatchlistItem[]> {
  return fetchJson<WatchlistItem[]>('/api/watchlist', { signal })
}

export async function addToWatchlist(showId: number): Promise<WatchlistItem> {
  return fetchJson<WatchlistItem>(`/api/watchlist/${showId}`, { method: 'POST' })
}

export async function removeFromWatchlist(showId: number): Promise<void> {
  const response = await apiFetch(`/api/watchlist/${showId}`, { method: 'DELETE' })
  if (!response.ok) {
    throw new Error(`Failed to remove show from watchlist`)
  }
}
