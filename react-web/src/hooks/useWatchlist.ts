import { useCallback, useEffect, useState } from 'react'
import type { WatchlistItem } from '../types/shows'
import { getWatchlist, addToWatchlist, removeFromWatchlist } from '../api/shows'

export function useWatchlist() {
  const [items, setItems] = useState<WatchlistItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const refresh = useCallback((signal?: AbortSignal) => {
    setLoading(true)
    setError(null)
    getWatchlist(signal)
      .then(setItems)
      .catch((err) => {
        if (err instanceof DOMException && err.name === 'AbortError') return
        setError(err instanceof Error ? err.message : 'Failed to load watchlist')
      })
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => {
    const controller = new AbortController()
    refresh(controller.signal)
    return () => controller.abort()
  }, [refresh])

  const add = useCallback(async (showId: number) => {
    const item = await addToWatchlist(showId)
    setItems((prev) => [item, ...prev.filter((i) => i.show_id !== item.show_id)])
    return item
  }, [])

  const remove = useCallback(async (showId: number) => {
    await removeFromWatchlist(showId)
    setItems((prev) => prev.filter((i) => i.show_id !== showId))
  }, [])

  const isOnWatchlist = useCallback(
    (showId: number) => items.some((i) => i.show_id === showId),
    [items],
  )

  return { items, loading, error, add, remove, isOnWatchlist }
}
