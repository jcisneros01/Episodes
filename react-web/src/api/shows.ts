import { fetchJson } from './client'
import type { TvShowSearchResponse, TvShowResponse, TvSeasonResponse } from '../types/shows'

export async function searchShows(
  query: string,
  page = 1,
  signal?: AbortSignal,
): Promise<TvShowSearchResponse> {
  const params = new URLSearchParams({ query, page: String(page) })
  return fetchJson<TvShowSearchResponse>(`/api/shows/search?${params}`, { signal })
}

export async function getShow(
  tvShowId: number,
  signal?: AbortSignal,
): Promise<TvShowResponse> {
  return fetchJson<TvShowResponse>(`/api/shows/${tvShowId}`, { signal })
}

export async function getSeasonEpisodes(
  tvShowId: number,
  seasonNumber: number,
  signal?: AbortSignal,
): Promise<TvSeasonResponse> {
  return fetchJson<TvSeasonResponse>(`/api/shows/${tvShowId}/season/${seasonNumber}`, { signal })
}
