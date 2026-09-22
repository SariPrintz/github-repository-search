import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';

export interface LoginResponse {
  token: string;
  username: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  readonly token = signal<string | null>(sessionStorage.getItem('jwt_token'));
  readonly username = signal<string | null>(sessionStorage.getItem('logged_in_username'));

  constructor(private readonly http: HttpClient) {}

  isAuthenticated(): boolean {
    return !!this.token();
  }

  login(username: string) {
    return this.http.post<LoginResponse>('/api/Auth/login', { username }).pipe(
      tap((response) => {
        this.token.set(response.token);
        this.username.set(response.username);
        sessionStorage.setItem('jwt_token', response.token);
        sessionStorage.setItem('logged_in_username', response.username);
      }),
    );
  }
}
