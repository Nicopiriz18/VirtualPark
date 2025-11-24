import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';
import { RegisterAccessRequest } from '../models/register-access-request';
import { Incidence } from '../models/incidence';
import { CreateIncidenceRequest } from '../models/create-incidence-request';

@Injectable({
  providedIn: 'root',
})
export class AttractionAccessService {
  private readonly baseUrl = `${environment.apiUrl}/api/AttractionAccess`;

  private http = inject(HttpClient);

  async registerAccess(attractionId: string, request: RegisterAccessRequest): Promise<unknown | null> {
    try {
      const res = await firstValueFrom(this.http.post<unknown>(`${this.baseUrl}/${attractionId}/access`, request));
      return res ?? null;
    } catch (error) {
      return null;
    }
  }

  async registerExit(attractionId: string, visitorId: string): Promise<unknown | null> {
    try {
      const res = await firstValueFrom(
        this.http.put<unknown>(`${this.baseUrl}/${attractionId}/exit`, {}, { params: { visitorId } }),
      );
      return res ?? null;
    } catch (error) {
      return null;
    }
  }

  async getCapacity(attractionId: string): Promise<{ attractionId: string; currentOccupancy: number; remainingCapacity: number } | null> {
    try {
      // controller defines GET("capacity") but expects attractionId as parameter - send as query string
      const res = await firstValueFrom(this.http.get<{ attractionId: string; currentOccupancy: number; remainingCapacity: number }>(`${this.baseUrl}/capacity`, { params: { attractionId } }));
      return res ?? null;
    } catch (error) {
      return null;
    }
  }

  // Incidents
  async createIncident(attractionId: string, request: CreateIncidenceRequest): Promise<Incidence | null> {
    try {
      const res = await firstValueFrom(this.http.post<Incidence>(`${this.baseUrl}/${attractionId}/incidents`, request));
      return res ?? null;
    } catch (error) {
      return null;
    }
  }

  async closeIncident(attractionId: string, incidentId: string): Promise<boolean> {
    try {
      await firstValueFrom(
        this.http.put<void>(`${this.baseUrl}/${attractionId}/incidents`, {}, { params: { incidentId, action: 'close' } }),
      );
      return true;
    } catch (error) {
      return false;
    }
  }

  async reopenIncident(attractionId: string, incidentId: string): Promise<boolean> {
    try {
      await firstValueFrom(
        this.http.put<void>(`${this.baseUrl}/${attractionId}/incidents`, {}, { params: { incidentId, action: 'reopen' } }),
      );
      return true;
    } catch (error) {
      return false;
    }
  }

  async getIncident(attractionId: string, incidentId: string): Promise<Incidence | null> {
    try {
      const res = await firstValueFrom(
        this.http.get<Incidence>(`${this.baseUrl}/${attractionId}/incidents`, { params: { incidentId } }),
      );
      return res ?? null;
    } catch (error) {
      return null;
    }
  }

  async getIncidents(attractionId: string): Promise<Incidence[] | null> {
    try {
      const res = await firstValueFrom(this.http.get<Incidence[]>(`${this.baseUrl}/${attractionId}/incidents`));
      return res ?? null;
    } catch (error) {
      return null;
    }
  }

  async getAttractionStatus(attractionId: string): Promise<{ attractionId: string; isOutOfService: boolean } | null> {
    try {
      const res = await firstValueFrom(this.http.get<{ attractionId: string; isOutOfService: boolean }>(`${this.baseUrl}/${attractionId}/status`));
      return res ?? null;
    } catch (error) {
      return null;
    }
  }
}
