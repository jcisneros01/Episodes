import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [DecimalPipe],
  template: `
    @if (totalPages > 1) {
      <div class="flex items-center justify-between gap-4">
        <p class="text-sm text-gray-500">
          {{ totalResults | number }} result{{ totalResults !== 1 ? 's' : '' }}
        </p>
        <div class="flex items-center gap-2">
          <button
            type="button"
            (click)="pageChange.emit(page - 1)"
            [disabled]="page <= 1"
            class="flex items-center gap-1 rounded-lg border border-gray-700 bg-gray-800 px-3 py-1.5 text-sm text-gray-300 transition hover:bg-gray-700 disabled:cursor-not-allowed disabled:opacity-40"
          >
            <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 19.5 8.25 12l7.5-7.5" />
            </svg>
            Prev
          </button>
          <span class="text-sm text-gray-400">{{ page }} / {{ totalPages }}</span>
          <button
            type="button"
            (click)="pageChange.emit(page + 1)"
            [disabled]="page >= totalPages"
            class="flex items-center gap-1 rounded-lg border border-gray-700 bg-gray-800 px-3 py-1.5 text-sm text-gray-300 transition hover:bg-gray-700 disabled:cursor-not-allowed disabled:opacity-40"
          >
            Next
            <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="m8.25 4.5 7.5 7.5-7.5 7.5" />
            </svg>
          </button>
        </div>
      </div>
    }
  `,
})
export class PaginationComponent {
  @Input() page = 1;
  @Input() totalPages = 0;
  @Input() totalResults = 0;
  @Output() pageChange = new EventEmitter<number>();
}
