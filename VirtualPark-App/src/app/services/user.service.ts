import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly privateUsersSignal = signal<User[]>([]);
  readonly usersSignal = this.privateUsersSignal.asReadonly();

  private readonly baseUrl = `${environment.apiUrl}/api/Users`;

  private http = inject(HttpClient);

  constructor() {
    this.fetchUsers();
  }

  async fetchUsers(): Promise<User[]> {
    try {
      const data = await firstValueFrom(this.http.get<User[]>(this.baseUrl));
      this.privateUsersSignal.set(data ?? []);
      return data ?? [];
    } catch (error) {
      this.privateUsersSignal.set([]);
      return [];
    }
  }

  async fetchUserById(id: string): Promise<User | null> {
    try {
      const user = await firstValueFrom(this.http.get<User>(`${this.baseUrl}/${id}`));
      return user ?? null;
    } catch (error) {
      return null;
    }
  }

  async createUser(user: {
    name: string;
    surname: string;
    email: string;
    password: string;
    role: string;
    birthDate?: string;
  }): Promise<User | null> {
    try {
      const created = await firstValueFrom(this.http.post<User>(this.baseUrl, user));
      if (!created) return null;
      this.privateUsersSignal.update((users) => [...users, created]);
      return created;
    } catch (error) {
      return null;
    }
  }

  async updateUser(
    id: string,
    updated: Partial<User> & { password?: string }
  ): Promise<boolean> {
    try {
      // Solo enviar los campos válidos para el backend
      const body: any = {
        name: updated.name,
        surname: updated.surname,
        email: updated.email,
      };
      if (updated.password) {
        body.password = updated.password;
      }
      await firstValueFrom(this.http.put<void>(`${this.baseUrl}/${id}`, body));
      this.privateUsersSignal.update((users) =>
        users.map((u) => (u.id === id ? { ...u, ...updated } : u))
      );
      return true;
    } catch (error) {
      return false;
    }
  }

  async deleteUser(id: string): Promise<boolean> {
    try {
      await firstValueFrom(this.http.delete(`${this.baseUrl}/${id}`));
      this.privateUsersSignal.update((users) => users.filter((u) => u.id !== id));
      return true;
    } catch (error) {
      return false;
    }
  }

  async assignRole(id: string, role: string, birthDate?: string): Promise<boolean> {
    try {
      const body: { role: string; birthDate?: string } = { role };
      if (birthDate) {
        body.birthDate = birthDate;
      }
      await firstValueFrom(this.http.post(`${this.baseUrl}/${id}/roles`, body));
      // Opcional: actualizar el usuario en el signal si es necesario
      return true;
    } catch (error) {
      return false;
    }
  }
}
