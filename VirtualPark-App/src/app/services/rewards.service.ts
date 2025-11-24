import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';

export interface Reward {
  id: string;
  name: string;
  description: string;
  costInPoints: number;
  availableQuantity: number;
  requiredLevel: string | null;
}

export interface CreateRewardRequest {
  name: string;
  description: string;
  costInPoints: number;
  availableQuantity: number;
  requiredLevel?: string | null;
}

@Injectable({ providedIn: 'root' })
export class RewardsService {
  private readonly baseUrl = `${environment.apiUrl}/api/Reward`;
  private readonly redemptionUrl = `${environment.apiUrl}/api/RewardRedemption`;
  private http = inject(HttpClient);

  async create(request: CreateRewardRequest): Promise<Reward | null> {
    try {
      const res = await firstValueFrom(
        this.http.post<Reward>(this.baseUrl, request)
      );
      return res ?? null;
    } catch (error) {
      console.error('Failed to create reward', error);
      return null;
    }
  }

  async list(): Promise<Reward[]> {
    try {
      const res = await firstValueFrom(this.http.get<Reward[]>(this.baseUrl));
      return res ?? [];
    } catch (error) {
      console.error('Failed to fetch rewards', error);
      return [];
    }
  }

  async redeem(rewardId: string): Promise<boolean> {
    try {
      await firstValueFrom(this.http.post(`${this.baseUrl}/${rewardId}/redeem`, {}));
      return true;
    } catch (error) {
      console.error('Failed to redeem reward', error);
      return false;
    }
  }

  async getRedemptions(visitorId: string): Promise<Array<{ id: string; rewardId: string; pointsSpent: number; date: string }>> {
    try {
      const res = await firstValueFrom(
        this.http.get<Array<{ id: string; rewardId: string; pointsSpent: number; date: string }>>(
          `${this.redemptionUrl}/visitor/${visitorId}`
        )
      );
      return res ?? [];
    } catch (error) {
      console.error('Failed to fetch reward redemptions', error);
      return [];
    }
  }
}
