import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RewardsService, Reward } from '../services/rewards.service';
import { FeedbackService } from '../services/feedback.service';
import { markAllAsTouched } from '../shared/utils/forms.util';

type MembershipLevel = 'Standard' | 'Premium' | 'VIP';

@Component({
  selector: 'app-reward-management',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './reward-management.component.html',
})
export class RewardManagementComponent implements OnInit {
  private fb = inject(FormBuilder);
  private rewardsService = inject(RewardsService);
  private feedback = inject(FeedbackService);

  membershipLevels: MembershipLevel[] = ['Standard', 'Premium', 'VIP'];
  displayedColumns = ['name', 'cost', 'stock', 'level'];

  private loading = signal(false);
  private creating = signal(false);

  rewards = signal<Reward[]>([]);

  readonly defaultFormValue = {
    name: '',
    description: '',
    costInPoints: 1,
    availableQuantity: 1,
    requiredLevel: null as MembershipLevel | null,
  };

  form = this.fb.group({
    name: ['', Validators.required],
    description: ['', Validators.required],
    costInPoints: [1, [Validators.required, Validators.min(1)]],
    availableQuantity: [1, [Validators.required, Validators.min(1)]],
    requiredLevel: [this.defaultFormValue.requiredLevel],
  });

  ngOnInit(): void {
    void this.loadRewards();
  }

  isLoading = () => this.loading();
  isCreating = () => this.creating();

  async loadRewards(): Promise<void> {
    this.loading.set(true);
    try {
      const list = await this.rewardsService.list();
      this.rewards.set(list);
    } catch {
      this.rewards.set([]);
    } finally {
      this.loading.set(false);
    }
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }

    const value = this.form.getRawValue();
    this.creating.set(true);
    try {
      const reward = await this.rewardsService.create({
        name: value.name!.trim(),
        description: value.description!.trim(),
        costInPoints: Number(value.costInPoints ?? 0),
        availableQuantity: Number(value.availableQuantity ?? 0),
        requiredLevel: value.requiredLevel ?? null,
      });
      if (reward) {
        this.feedback.success('La recompensa se creó correctamente.');
        this.rewards.update(list => [reward, ...list]);
        this.form.reset(this.defaultFormValue);
      } else {
        this.feedback.error('No se pudo crear la recompensa. Intenta nuevamente.');
      }
    } finally {
      this.creating.set(false);
    }
  }
}
