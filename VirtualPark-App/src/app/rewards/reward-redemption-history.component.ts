import { Component, inject, signal } from '@angular/core';
import { NgIf, NgFor, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RewardsService } from '../services/rewards.service';
import { AuthService } from '../services/auth.service';

@Component({
  standalone: true,
  selector: 'app-reward-redemption-history',
  imports: [NgIf, NgFor, FormsModule, DatePipe],
  templateUrl: './reward-redemption-history.component.html',
})
export class RewardRedemptionHistoryComponent {
  private rewardsService = inject(RewardsService);
  private authService = inject(AuthService);

  redemptions = signal<Array<{ id: string; rewardId: string; pointsSpent: number; date: string }>>([]);
  isLoading = signal(false);

  constructor() {
    this.load();
  }

  async load(): Promise<void> {
    const userId = this.authService.getUserId();
    if (!userId) return;
    this.isLoading.set(true);
    this.redemptions.set(await this.rewardsService.getRedemptions(userId));
    this.isLoading.set(false);
  }
}
