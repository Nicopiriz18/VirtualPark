import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';

export interface AttractionUsageReport {
  attractionId: string;
  attractionName: string;
  attractionType: string;
  visitCount: number;
  startDate: string;
  endDate: string;
  capacity: number;
  occupancyRate: number;
  averageVisitsPerDay: number;
  peakDayVisits: number;
  peakDay: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class ReportsService {
  private readonly baseUrl = `${environment.apiUrl}/api/Reports`;
  private http = inject(HttpClient);

  async getAttractionUsage(startDate: string, endDate: string): Promise<AttractionUsageReport[]> {
    try {
      const params = new HttpParams()
        .set('startDate', this.toIsoDate(startDate))
        .set('endDate', this.toIsoDate(endDate));
      const res = await firstValueFrom(
        this.http.get<AttractionUsageReport[]>(`${this.baseUrl}/attraction-usage`, { params })
      );
      return res ?? [];
    } catch (error) {
      console.error('Failed to load attraction usage report', error);
      return [];
    }
  }

  private toIsoDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
      return new Date().toISOString();
    }
    return date.toISOString();
  }
}
