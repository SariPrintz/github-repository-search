import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { RepositoryItem } from './repository.service';

@Injectable({
  providedIn: 'root',
})
export class BookmarkService {
  constructor(private readonly http: HttpClient) {}

  getBookmarks(): Observable<RepositoryItem[]> {
    return this.http.get<RepositoryItem[]>('/api/Bookmarks');
  }

  addBookmark(repository: RepositoryItem): Observable<RepositoryItem> {
    return this.http.post<RepositoryItem>('/api/Bookmarks', repository);
  }

  removeBookmark(htmlUrl: string): Observable<void> {
    const params = new HttpParams().set('htmlUrl', htmlUrl);

    return this.http.delete<void>('/api/Bookmarks', { params });
  }
}
