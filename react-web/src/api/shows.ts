import type { TvShowSearchResponse } from '../types/shows'

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
