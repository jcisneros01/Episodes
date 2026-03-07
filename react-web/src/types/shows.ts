export interface TvSearchResult {
  id: number
  name: string
  poster_path: string
  overview: string
}

export interface TvShowSearchResponse {
  results: TvSearchResult[]
  page: number
  total_pages: number
  total_results: number
}

export interface TvSeasonSummary {
  id: number
  name: string
  season_number: number
  episode_count: number
}

export interface TvShowResponse {
  id: number
  name: string
  poster_path: string
  overview: string
  first_air_date: string
  in_production: boolean
  networks: string[]
  genres: string[]
  status: string
  seasons: TvSeasonSummary[]
  number_of_episodes: number
  number_of_seasons: number
}

export interface Episode {
  name: string
  overview: string
  air_date: string
  episode_number: number
}

export interface TvSeasonResponse {
  name: string
  overview: string
  season_number: number
  episodes: Episode[]
}
