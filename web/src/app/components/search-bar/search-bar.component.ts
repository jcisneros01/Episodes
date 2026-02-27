import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  template: `
    <div class="relative w-full max-w-2xl">
      <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-4">
        <svg class="h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round" d="M21 21l-4.35-4.35M17 11A6 6 0 1 1 5 11a6 6 0 0 1 12 0z" />
        </svg>
      </div>
      <input
        type="text"
        [value]="value"
        (input)="valueChange.emit($any($event.target).value)"
        placeholder="Search TV shows..."
        class="w-full rounded-xl border border-gray-700 bg-gray-800 py-3 pl-12 pr-4 text-gray-100 placeholder-gray-500 shadow-sm outline-none transition focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500/40"
        autofocus
      />
      @if (value) {
        <button
          type="button"
          (click)="valueChange.emit('')"
          class="absolute inset-y-0 right-0 flex items-center pr-4 text-gray-500 hover:text-gray-300"
          aria-label="Clear search"
        >
          <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      }
    </div>
  `,
})
export class SearchBarComponent {
  @Input() value = '';
  @Output() valueChange = new EventEmitter<string>();
}
