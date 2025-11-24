import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';

export interface MaintenanceDto {
  id: string;
  attractionId: string;
  scheduledDate: string;
  startTime: string;
  estimatedDuration: string;
  description: string;
  associatedIncidenceId?: string | null;
}

export interface CreateMaintenanceRequest {
  attractionId: string;
  scheduledDate: string;
  startTime: string;
  estimatedDuration: string;
  description: string;
}

@Injectable({ providedIn: 'root' })
export class MaintenanceService {
  private readonly baseUrl = `${environment.apiUrl}/api/Maintenance`;
  private http = inject(HttpClient);

  async schedule(request: CreateMaintenanceRequest): Promise<MaintenanceDto | null> {
    try {
      const res = await firstValueFrom(
        this.http.post<MaintenanceDto>(this.baseUrl, request)
      );
      return res ?? null;
    } catch (error) {
      console.error('Failed to schedule maintenance', error);
      return null;
    }
  }

  async getByAttraction(attractionId: string): Promise<MaintenanceDto[]> {
    try {
      const res = await firstValueFrom(
        this.http.get<MaintenanceDto[]>(`${this.baseUrl}/attraction/${attractionId}`)
      );
      return res ?? [];
    } catch (error) {
      console.error('Failed to load maintenance list', error);
      return [];
    }
  }

  async delete(id: string): Promise<boolean> {
    try {
      await firstValueFrom(this.http.delete<void>(`${this.baseUrl}/${id}`));
      return true;
    } catch (error) {
      console.error('Failed to delete maintenance', error);
      return false;
    }
  }
}
