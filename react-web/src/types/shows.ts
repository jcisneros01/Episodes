export interface TvSearchResult {
  id: number
  name: string
  overview: string | null
  poster_path: string | null
}

export interface TvShowSearchResponse {
  page: number
  total_pages: number
  total_results: number
  results: TvSearchResult[]
}

export interface TvSeasonSummary {
  id: number
  season_number: number
  name: string
  overview: string | null
  air_date: string | null
  episode_count: number
  poster_path: string | null
}

export interface Episode {
  id: number
  episode_number: number
  name: string
  overview: string | null
  air_date: string | null
  is_watched: boolean
}

export interface TvSeasonResponse {
  id: number
  name: string
  season_number: number
  overview: string | null
  air_date: string | null
  episodes: Episode[]
}

export interface TvShowResponse {
  id: number
  name: string
  overview: string | null
  poster_path: string | null
  first_air_date: string | null
  status: string
  number_of_seasons: number
  number_of_episodes: number
  genres: string[]
  networks: string[]
  seasons: TvSeasonSummary[]
}

export interface WatchlistItem {
  show_id: number
  name: string
  poster_img_link: string | null
  status: string
  added_at: string
}
