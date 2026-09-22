import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface RepositoryOwner {
  login: string;
  avatar_url: string;
}

export interface RepositoryItem {
  name: string;
  owner: RepositoryOwner;
  html_url: string;
  description: string | null;
  stargazers_count: number;
}

export interface RepositorySearchResponse {
  total_count: number;
  items: RepositoryItem[];
}

@Injectable({
  providedIn: 'root',
})
export class RepositoryService {
  constructor(private readonly http: HttpClient) {}

  searchRepositories(keyword: string): Observable<RepositorySearchResponse> {
    const params = new HttpParams().set('keyword', keyword);

    return this.http.get<RepositorySearchResponse>('/api/Repositories/search', {
      params,
    });
  }
}
