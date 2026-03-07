interface PaginationProps {
  page: number
  totalPages: number
  totalResults: number
  onPageChange: (page: number) => void
}

export function Pagination({ page, totalPages, totalResults, onPageChange }: PaginationProps) {
  if (totalPages <= 1) {
    return null
  }

  return (
    <div className="flex flex-col items-center justify-between gap-4 rounded-xl border border-gray-800 bg-gray-900 px-4 py-4 sm:flex-row">
      <p className="text-sm text-gray-400">{totalResults.toLocaleString()} results</p>

      <div className="flex items-center gap-3">
        <button
          type="button"
          onClick={() => onPageChange(page - 1)}
          disabled={page <= 1}
          className="rounded-lg border border-gray-700 px-3 py-2 text-sm text-gray-200 transition hover:border-indigo-500 hover:text-indigo-300 disabled:cursor-not-allowed disabled:opacity-40"
        >
          Previous
        </button>

        <span className="text-sm text-gray-300">
          Page {page} of {totalPages}
        </span>

        <button
          type="button"
          onClick={() => onPageChange(page + 1)}
          disabled={page >= totalPages}
          className="rounded-lg border border-gray-700 px-3 py-2 text-sm text-gray-200 transition hover:border-indigo-500 hover:text-indigo-300 disabled:cursor-not-allowed disabled:opacity-40"
        >
          Next
        </button>
      </div>
    </div>
  )
}
