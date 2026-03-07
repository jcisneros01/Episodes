import { useCallback, useEffect, useRef, useState } from 'react'
import { searchShows } from '../api/shows'
import type { TvShowSearchResponse } from '../types/shows'

const DEBOUNCE_MS = 400

interface SearchState {
  data: TvShowSearchResponse | null
  loading: boolean
  error: string | null
}

export function useSearch() {
  const [query, setQuery] = useState('')
  const [page, setPage] = useState(1)
  const [state, setState] = useState<SearchState>({
    data: null,
    loading: false,
    error: null,
  })

  const debounceTimer = useRef<ReturnType<typeof setTimeout> | null>(null)
  const abortController = useRef<AbortController | null>(null)

  const doSearch = useCallback(async (q: string, p: number) => {
    abortController.current?.abort()
    const controller = new AbortController()
    abortController.current = controller

    setState((prev) => ({ ...prev, loading: true, error: null }))

    try {
      const data = await searchShows(q, p, controller.signal)
      setState({ data, loading: false, error: null })
    } catch (err) {
      if (err instanceof Error && err.name === 'AbortError') return
      const message = err instanceof Error ? err.message : 'Something went wrong'
      setState({ data: null, loading: false, error: message })
    }
  }, [])

  useEffect(() => {
    if (debounceTimer.current) clearTimeout(debounceTimer.current)

    if (!query.trim()) {
      abortController.current?.abort()
      setState({ data: null, loading: false, error: null })
      return
    }

    debounceTimer.current = setTimeout(() => {
      void doSearch(query, page)
    }, DEBOUNCE_MS)

    return () => {
      if (debounceTimer.current) clearTimeout(debounceTimer.current)
    }
  }, [query, page, doSearch])

  const handleQueryChange = (q: string) => {
    setQuery(q)
    setPage(1)
  }

  const handlePageChange = (newPage: number) => {
    setPage(newPage)
  }

  return {
    query,
    page,
    ...state,
    handleQueryChange,
    handlePageChange,
  }
}
