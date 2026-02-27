import { Component, Input } from '@angular/core';
import { TvShowSearchResponse } from '../../models/shows';
import { ShowCardComponent } from '../show-card/show-card.component';

@Component({
  selector: 'app-show-grid',
  standalone: true,
  imports: [ShowCardComponent],
  template: `
    @if (loading) {
      <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6">
        @for (item of skeletons; track item) {
          <div class="overflow-hidden rounded-xl bg-gray-800">
            <div class="aspect-[2/3] animate-pulse bg-gray-700"></div>
            <div class="space-y-2 p-3">
              <div class="h-4 animate-pulse rounded bg-gray-700"></div>
              <div class="h-3 w-3/4 animate-pulse rounded bg-gray-700"></div>
              <div class="h-3 animate-pulse rounded bg-gray-700"></div>
            </div>
          </div>
        }
      </div>
    } @else if (error) {
      <div class="flex flex-col items-center gap-3 py-16 text-center">
        <svg class="h-12 w-12 text-red-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z" />
        </svg>
        <p class="text-gray-300">{{ error }}</p>
      </div>
    } @else if (!data || !query.trim()) {
      <div class="flex flex-col items-center gap-3 py-16 text-center">
        <svg class="h-16 w-16 text-gray-600" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1">
          <path stroke-linecap="round" stroke-linejoin="round" d="M6 20.25h12m-7.5-3v3m-4.875 0H5.625c-.621 0-1.125-.504-1.125-1.125V4.875C4.5 4.254 5.004 3.75 5.625 3.75h12.75c.621 0 1.125.504 1.125 1.125v11.25c0 .621-.504 1.125-1.125 1.125h-2.25M8.25 8.25h7.5M8.25 12h4.5" />
        </svg>
        <p class="text-lg font-medium text-gray-400">Search for a TV show to get started</p>
      </div>
    } @else if (data.results.length === 0) {
      <div class="flex flex-col items-center gap-3 py-16 text-center">
        <p class="text-lg font-medium text-gray-400">
          No results for <span class="text-gray-200">"{{ query }}"</span>
        </p>
      </div>
    } @else {
      <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6">
        @for (show of data.results; track show.id) {
          <app-show-card [show]="show" />
        }
      </div>
    }
  `,
})
export class ShowGridComponent {
  @Input() data: TvShowSearchResponse | null = null;
  @Input() loading = false;
  @Input() error: string | null = null;
  @Input() query = '';

  readonly skeletons = Array.from({ length: 12 }, (_, i) => i);
}
