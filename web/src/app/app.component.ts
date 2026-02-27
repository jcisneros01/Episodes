import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject } from 'rxjs';
import { catchError, debounceTime, of, switchMap } from 'rxjs';
import { ShowsService } from './services/shows.service';
import { TvShowSearchResponse } from './models/shows';
import { SearchBarComponent } from './components/search-bar/search-bar.component';
import { ShowGridComponent } from './components/show-grid/show-grid.component';
import { PaginationComponent } from './components/pagination/pagination.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [SearchBarComponent, ShowGridComponent, PaginationComponent],
  template: `
    <div class="min-h-screen bg-gray-950 text-gray-100">
      <header class="border-b border-gray-800 bg-gray-900 px-6 py-4">
        <div class="mx-auto flex max-w-7xl items-center gap-6">
          <h1 class="text-xl font-bold tracking-tight text-indigo-400">Episodes</h1>
          <app-search-bar [value]="query()" (valueChange)="onQueryChange($event)" />
        </div>
      </header>

      <main class="mx-auto max-w-7xl space-y-6 px-6 py-8">
        <app-show-grid
          [data]="data()"
          [loading]="loading()"
          [error]="error()"
          [query]="query()"
        />

        @if (data()) {
          <app-pagination
            [page]="page()"
            [totalPages]="data()!.total_pages"
            [totalResults]="data()!.total_results"
            (pageChange)="onPageChange($event)"
          />
        }
      </main>
    </div>
  `,
})
export class AppComponent {
  private showsService = inject(ShowsService);
  private destroyRef = inject(DestroyRef);

  query = signal('');
  page = signal(1);
  loading = signal(false);
  error = signal<string | null>(null);
  data = signal<TvShowSearchResponse | null>(null);

  private search$ = new Subject<{ query: string; page: number }>();

  constructor() {
    this.search$
      .pipe(
        debounceTime(400),
        switchMap(({ query, page }) => {
          if (!query.trim()) {
            this.data.set(null);
            this.loading.set(false);
            return of(null);
          }
          this.loading.set(true);
          this.error.set(null);
          return this.showsService.search(query, page).pipe(
            catchError((err: Error) => {
              this.error.set(err.message ?? 'Something went wrong');
              this.data.set(null);
              return of(null);
            }),
          );
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((result) => {
        this.loading.set(false);
        if (result !== null) {
          this.data.set(result);
        }
      });
  }

  onQueryChange(q: string): void {
    this.query.set(q);
    this.page.set(1);
    this.search$.next({ query: q, page: 1 });
  }

  onPageChange(p: number): void {
    this.page.set(p);
    this.search$.next({ query: this.query(), page: p });
  }
}
