interface PaginationProps {
  page: number
  totalPages: number
  totalResults: number
  onPageChange: (page: number) => void
}

export function Pagination({ page, totalPages, totalResults, onPageChange }: PaginationProps) {
  if (totalPages <= 1) return null

  return (
    <div className="flex items-center justify-between gap-4">
      <p className="text-sm text-gray-500">
        {totalResults.toLocaleString()} result{totalResults !== 1 ? 's' : ''}
      </p>
      <div className="flex items-center gap-2">
        <button
          type="button"
          onClick={() => onPageChange(page - 1)}
          disabled={page <= 1}
          className="flex items-center gap-1 rounded-lg border border-gray-700 bg-gray-800 px-3 py-1.5 text-sm text-gray-300 transition hover:bg-gray-700 disabled:cursor-not-allowed disabled:opacity-40"
        >
          <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 19.5 8.25 12l7.5-7.5" />
          </svg>
          Prev
        </button>
        <span className="text-sm text-gray-400">
          {page} / {totalPages}
        </span>
        <button
          type="button"
          onClick={() => onPageChange(page + 1)}
          disabled={page >= totalPages}
          className="flex items-center gap-1 rounded-lg border border-gray-700 bg-gray-800 px-3 py-1.5 text-sm text-gray-300 transition hover:bg-gray-700 disabled:cursor-not-allowed disabled:opacity-40"
        >
          Next
          <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            <path strokeLinecap="round" strokeLinejoin="round" d="m8.25 4.5 7.5 7.5-7.5 7.5" />
          </svg>
        </button>
      </div>
    </div>
  )
}
