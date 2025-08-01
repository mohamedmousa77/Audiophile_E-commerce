import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { ApiService } from '../api/api.service';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

interface AuthResponse {
  token: string;
}

@Injectable({providedIn: 'root'})

export class AuthService {

  private tokenKey = 'token';
  private apiUrl = '';
  private authStatus = new BehaviorSubject<boolean>(this.hasToken());

  constructor(
    private http: HttpClient,
    private apiServices: ApiService,
    private router: Router
  ) {
    this.apiUrl = this.apiServices.getBaseUrl + '/auth';
  }

  login(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, { email, password });
  }

  register(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): Observable<boolean> {
    return this.authStatus.asObservable();
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.authStatus.next(false);
    this.router.navigate(['/']);
  }

  storeToken(token: string) {
    localStorage.setItem(this.tokenKey, token);
    this.authStatus.next(true);
  }

  getUserIdFromToken(): number | null {
    const token = this.getToken();
    if (!token) return null;
    try{ 
      const decoded: any = jwtDecode(token);
      return decoded?.id ?? null;
    } catch (e){ 
      console.error('Invalid token', e);
      return null;
    }
  }
}
