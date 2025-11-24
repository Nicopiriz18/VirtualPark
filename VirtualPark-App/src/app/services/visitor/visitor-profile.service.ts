import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

export interface UpdateVisitorProfileRequest {
  name: string;
  surname: string;
  email: string;
  password?: string | null;
}

@Injectable({ providedIn: 'root' })
export class VisitorProfileService {
  private readonly baseUrl = `${environment.apiUrl}/api/Visitors`;
  private http = inject(HttpClient);

  async updateProfile(id: string, request: UpdateVisitorProfileRequest): Promise<boolean> {
    try {
      await firstValueFrom(this.http.put(`${this.baseUrl}/${id}`, request));
      return true;
    } catch (error) {
      console.error('Failed to update visitor profile', error);
      return false;
    }
  }
}
