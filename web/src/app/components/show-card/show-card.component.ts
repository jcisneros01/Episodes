import { Component, Input } from '@angular/core';
import { TvSearchResult } from '../../models/shows';

const TMDB_IMAGE_BASE = 'https://image.tmdb.org/t/p/w300';

@Component({
  selector: 'app-show-card',
  standalone: true,
  template: `
    <article class="flex flex-col overflow-hidden rounded-xl bg-gray-800 shadow-md transition hover:shadow-indigo-500/20 hover:ring-1 hover:ring-indigo-500/40">
      <div class="aspect-[2/3] w-full bg-gray-700">
        @if (show.poster_path) {
          <img
            [src]="imageBase + show.poster_path"
            [alt]="show.name + ' poster'"
            class="h-full w-full object-cover"
            loading="lazy"
          />
        } @else {
          <div class="flex h-full items-center justify-center">
            <svg class="h-12 w-12 text-gray-600" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1">
              <path stroke-linecap="round" stroke-linejoin="round" d="M3.375 19.5h17.25m-17.25 0a1.125 1.125 0 0 1-1.125-1.125M3.375 19.5h1.5C5.496 19.5 6 18.996 6 18.375m-3.75.125v-5.625M5.25 4.5h13.5A2.25 2.25 0 0 1 21 6.75v8.25A2.25 2.25 0 0 1 18.75 17.25H5.25A2.25 2.25 0 0 1 3 15V6.75A2.25 2.25 0 0 1 5.25 4.5Z" />
            </svg>
          </div>
        }
      </div>
      <div class="flex flex-1 flex-col gap-1.5 p-3">
        <h3 class="line-clamp-2 text-sm font-semibold text-gray-100">{{ show.name }}</h3>
        @if (show.overview) {
          <p class="line-clamp-3 text-xs leading-relaxed text-gray-400">{{ show.overview }}</p>
        }
      </div>
    </article>
  `,
})
export class ShowCardComponent {
  @Input({ required: true }) show!: TvSearchResult;
  readonly imageBase = TMDB_IMAGE_BASE;
}
