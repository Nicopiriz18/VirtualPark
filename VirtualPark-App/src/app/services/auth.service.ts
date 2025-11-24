import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';

interface LoginRequest {
  email: string;
  password: string;
}

interface AuthResultDto {
  token: string;
  roles: string[];
  userId?: string;
  email?: string | null;
}

interface RegisterRequest {
  name: string;
  surname: string;
  email: string;
  password: string;
  birthday: string; // ISO string
}

interface RegisterResponseDto {
  id: string;
  fullName: string;
  email: string | null;
}

export interface UserDto {
  id: string;
  name: string;
  surname: string;
  email: string;
  roles: string[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = `${environment.apiUrl}/api/Auth`;
  private readonly tokenKey = 'auth_token';
  private readonly rolesKey = 'roles';
  private readonly userIdKey = 'user_id';
  private readonly userEmailKey = 'user_email';

  private http = inject(HttpClient);

  async register(request: RegisterRequest): Promise<RegisterResponseDto | null> {
    try {
      const body = {
        ...request,
        birthday: this.ensureIsoDate(request.birthday),
      };
      const res = await firstValueFrom(
        this.http.post<RegisterResponseDto>(`${this.baseUrl}/register`, body)
      );
      return res ?? null;
    } catch (e) {
      console.error('Register failed', e);
      return null;
    }
  }

  async login(email: string, password: string): Promise<string | null> {
    try {
      const body: LoginRequest = { email, password };
      const res = await firstValueFrom(
        this.http.post<AuthResultDto>(`${this.baseUrl}/login`, body)
      );
      const token = res?.token ?? null;
      if (!token) return null;
      localStorage.setItem(this.tokenKey, token);
      if (res?.roles) {
        localStorage.setItem(this.rolesKey, JSON.stringify(res.roles));
      } else {
        localStorage.removeItem(this.rolesKey);
      }
      if (res?.userId) {
        localStorage.setItem(this.userIdKey, res.userId);
      } else {
        localStorage.removeItem(this.userIdKey);
      }
      if (res?.email) {
        localStorage.setItem(this.userEmailKey, res.email);
      } else {
        localStorage.removeItem(this.userEmailKey);
      }
      return token;
    } catch (e) {
      console.error('Login failed', e);
      return null;
    }
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.rolesKey);
    localStorage.removeItem(this.userIdKey);
    localStorage.removeItem(this.userEmailKey);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getRoles(): string[] {
    try {
      return JSON.parse(localStorage.getItem(this.rolesKey) ?? '[]');
    } catch {
      return [];
    }
  }

  hasRole(role: string): boolean {
    return this.getRoles().includes(role);
  }

  getUserId(): string | null {
    return localStorage.getItem(this.userIdKey);
  }

  getUserEmail(): string | null {
    return localStorage.getItem(this.userEmailKey);
  }

  private ensureIsoDate(value: string): string {
    if (!value) {
      return new Date().toISOString();
    }
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
      return new Date().toISOString();
    }
    return date.toISOString();
  }
}
