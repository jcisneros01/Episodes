import { useEffect, useState } from 'react'
import { getShow } from '../api/shows'
import type { TvShowResponse } from '../types/shows'

interface ShowDetailState {
  show: TvShowResponse | null
  loading: boolean
  error: string | null
}

export function useShowDetail(showId: number) {
  const [state, setState] = useState<ShowDetailState>({
    show: null,
    loading: true,
    error: null,
  })

  useEffect(() => {
    const controller = new AbortController()

    setState({ show: null, loading: true, error: null })

    getShow(showId, controller.signal)
      .then((show) => {
        setState({ show, loading: false, error: null })
      })
      .catch((err) => {
        if (err instanceof Error && err.name === 'AbortError') return
        const message = err instanceof Error ? err.message : 'Something went wrong'
        setState({ show: null, loading: false, error: message })
      })

    return () => controller.abort()
  }, [showId])

  return state
}
