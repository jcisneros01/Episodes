import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TvShowSearchResponse } from '../models/shows';

@Injectable({ providedIn: 'root' })
export class ShowsService {
  private http = inject(HttpClient);

  search(query: string, page = 1): Observable<TvShowSearchResponse> {
    const params = new HttpParams().set('query', query).set('page', page);
    return this.http.get<TvShowSearchResponse>('/api/shows/search', { params });
  }
}
