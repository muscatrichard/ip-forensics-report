import { HttpClient } from '@angular/common/http';
import { Service, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest, User } from '../models/auth.models';

const TOKEN_KEY = 'access_token';

@Service()
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/auth';

  private readonly _token = signal<string | null>(localStorage.getItem(TOKEN_KEY));

  readonly token = this._token.asReadonly();

  readonly isAuthenticated = computed(() => this._token() !== null);

  register(request: RegisterRequest): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/login`, request)
      .pipe(tap(({ token }) => this.setToken(token)));
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this._token.set(null);
  }

  private setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
    this._token.set(token);
  }
}
