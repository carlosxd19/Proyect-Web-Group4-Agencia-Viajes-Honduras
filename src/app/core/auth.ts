import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

type LoginRequest = { email: string; password: string };
type LoginResponse = { token: string };

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = 'https://localhost:7078/api/Auth'; // ajusta el puerto si tu API usa otro
  private tokenKey = 'token';

  constructor(private http: HttpClient) {}

  login(body: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.base}/login`, body).pipe(
      tap(res => localStorage.setItem(this.tokenKey, res.token))
    );
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
  }

  get token(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  get isLoggedIn(): boolean {
    return !!this.token;
  }
}
