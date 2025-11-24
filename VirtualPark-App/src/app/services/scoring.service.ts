import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';

export interface ScoringStrategiesResponse {
  availableStrategies: string[];
  activeStrategy: string | null;
}

export interface DailyRankingResponse {
  date: string;
  ranking: VisitorScore[];
}

export interface VisitorScore {
  visitorId: string;
  visitorName: string;
  totalPoints: number;
  position: number;
  accessCount: number;
  lastActivity: string | null;
}

@Injectable({ providedIn: 'root' })
export class ScoringService {
  private readonly baseUrl = `${environment.apiUrl}/api/Scoring`;
  private http = inject(HttpClient);

  async getStrategies(): Promise<ScoringStrategiesResponse> {
    try {
      const res = await firstValueFrom(
        this.http.get<ScoringStrategiesResponse>(`${this.baseUrl}/strategies`)
      );
      return {
        availableStrategies: res?.availableStrategies ?? [],
        activeStrategy: res?.activeStrategy ?? null,
      };
    } catch (error) {
      console.error('Failed to load scoring strategies', error);
      return { availableStrategies: [], activeStrategy: null };
    }
  }

  async setActiveStrategy(strategyName: string): Promise<boolean> {
    try {
      await firstValueFrom(
        this.http.put(`${this.baseUrl}/strategies/active`, { strategyName })
      );
      return true;
    } catch (error) {
      console.error('Failed to set active strategy', error);
      return false;
    }
  }

  async uploadStrategyPlugin(pluginFile: File): Promise<boolean> {
    try {
      const formData = new FormData();
      formData.append('plugin', pluginFile, pluginFile.name);
      await firstValueFrom(this.http.post(`${this.baseUrl}/strategies/plugins`, formData));
      return true;
    } catch (error) {
      console.error('Failed to upload strategy plugin', error);
      return false;
    }
  }

  async getDailyRanking(date: string): Promise<DailyRankingResponse | null> {
    try {
      const params = new HttpParams().set('date', date);
      const res = await firstValueFrom(
        this.http.get<DailyRankingResponse>(`${this.baseUrl}/ranking/daily`, { params })
      );
      return res ?? null;
    } catch (error) {
      console.error('Failed to load daily ranking', error);
      return null;
    }
  }
}
