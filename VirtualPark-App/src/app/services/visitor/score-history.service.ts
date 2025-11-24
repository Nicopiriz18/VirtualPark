import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ScoreHistoryEntry {
  id: string;
  date: string;
  pointsAwarded: number;
  origin: string;
  strategyUsed: string;
  attractionName?: string | null;
  description: string;
  rewardId?: string | null;
}

@Injectable({ providedIn: 'root' })
export class ScoreHistoryService {
  private readonly baseUrl = `${environment.apiUrl}/api/Visitors`;
  private http = inject(HttpClient);
  private readonly guidRegex =
    /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;

  async getVisitorHistory(visitorId: string): Promise<ScoreHistoryEntry[]> {
    const id = visitorId?.trim();
    if (!id) {
      return [];
    }

    if (!this.guidRegex.test(id)) {
      console.warn('ScoreHistoryService: provided visitor id is not a valid GUID.');
      return [];
    }

    try {
      const url = `${this.baseUrl}/${encodeURIComponent(id)}/score-history`;
      const result = await firstValueFrom(this.http.get<ScoreHistoryEntry[]>(url));
      return result ?? [];
    } catch (error) {
      console.error('Failed to load score history', error);
      return [];
    }
  }
}
