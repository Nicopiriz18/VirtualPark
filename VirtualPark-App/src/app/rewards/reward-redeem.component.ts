import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RewardsService, Reward } from '../services/rewards.service';
import { FeedbackService } from '../services/feedback.service';
import { markAllAsTouched } from '../shared/utils/forms.util';

@Component({
  selector: 'app-reward-redeem',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './reward-redeem.component.html',
})
export class RewardRedeemComponent implements OnInit {
  private fb = inject(FormBuilder);
  private rewardsService = inject(RewardsService);
  private feedback = inject(FeedbackService);

  private loading = signal(false);
  private redeeming = signal(false);

  rewards = signal<Reward[]>([]);

  readonly defaultFormValue = {
    rewardId: '',
    hint: '',
  };

  form = this.fb.group({
    rewardId: ['', Validators.required],
    hint: [''],
  });

  ngOnInit(): void {
    void this.loadRewards();
  }

  isLoading = () => this.loading();
  isRedeeming = () => this.redeeming();

  async loadRewards(): Promise<void> {
    this.loading.set(true);
    try {
      const list = await this.rewardsService.list();
      this.rewards.set(list.filter((reward) => reward.availableQuantity > 0));
    } finally {
      this.loading.set(false);
    }
  }

  async onRedeem(): Promise<void> {
    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }

    const { rewardId } = this.form.getRawValue();
    if (!rewardId) return;

    this.redeeming.set(true);
    try {
      const success = await this.rewardsService.redeem(rewardId.trim());
      if (success) {
        this.feedback.success('¡Canje realizado con éxito! Revisa tu historial para confirmar el beneficio.');
        this.form.reset(this.defaultFormValue);
        await this.loadRewards();
      } else {
        this.feedback.error('No se pudo completar el canje. Verifica el identificador e intenta nuevamente.');
      }
    } finally {
      this.redeeming.set(false);
    }
  }

  prefill(reward: Reward): void {
    this.form.patchValue({
      rewardId: reward.id,
      hint: reward.name,
    });
  }
}
