export interface TvSearchResult {
  id: number;
  name: string;
  poster_path: string;
  overview: string;
}

export interface TvShowSearchResponse {
  results: TvSearchResult[];
  page: number;
  total_pages: number;
  total_results: number;
}
