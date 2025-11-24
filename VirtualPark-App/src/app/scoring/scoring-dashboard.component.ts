import { Component, inject, signal } from '@angular/core';
import { CommonModule, NgIf } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ScoringService, VisitorScore } from '../services/scoring.service';
import { FeedbackService } from '../services/feedback.service';
import { markAllAsTouched } from '../shared/utils/forms.util';

@Component({
  selector: 'app-scoring-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NgIf,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './scoring-dashboard.component.html',
})
export class ScoringDashboardComponent {
  private scoringService = inject(ScoringService);
  private feedback = inject(FeedbackService);
  private fb = inject(FormBuilder);

  private loadingStrategies = signal(false);
  private loadingRanking = signal(false);
  private uploadingPlugin = signal(false);

  availableStrategies = signal<string[]>([]);
  activeStrategy = signal<string | null>(null);
  ranking = signal<VisitorScore[]>([]);
  selectedPlugin = signal<File | null>(null);

  rankingColumns: string[] = ['position', 'visitor', 'points'];

  rankingForm = this.fb.group({
    date: [this.today(), Validators.required],
  });

  constructor() {
    void this.loadStrategies();
    void this.onLoadRanking();
  }

  isLoadingStrategies = () => this.loadingStrategies();
  isLoadingRanking = () => this.loadingRanking();
  isUploadingPlugin = () => this.uploadingPlugin();

  async loadStrategies(): Promise<void> {
    this.loadingStrategies.set(true);
    try {
      const res = await this.scoringService.getStrategies();
      this.availableStrategies.set(res.availableStrategies);
      this.activeStrategy.set(res.activeStrategy);
    } finally {
      this.loadingStrategies.set(false);
    }
  }

  async onSetStrategy(strategy: string): Promise<void> {
    if (strategy === this.activeStrategy()) return;
    const confirmed = confirm(`¿Deseas activar la estrategia "${strategy}"?`);
    if (!confirmed) return;
    const success = await this.scoringService.setActiveStrategy(strategy);
    if (success) {
      this.feedback.success(`Estrategia "${strategy}" activada correctamente.`);
      this.activeStrategy.set(strategy);
    } else {
      this.feedback.error('No se pudo actualizar la estrategia seleccionada.');
    }
  }

  onPluginSelected(event: Event): void {
    const input = event.target as HTMLInputElement | null;
    if (!input) return;

    const file = input.files?.[0] ?? null;

    if (!file) {
      this.selectedPlugin.set(null);
      input.value = '';
      return;
    }

    if (!file.name.toLowerCase().endsWith('.dll')) {
      this.feedback.error('Selecciona un archivo con extensión .dll.');
      this.selectedPlugin.set(null);
      input.value = '';
      return;
    }

    this.selectedPlugin.set(file);
    input.value = '';
  }

  async onUploadPlugin(): Promise<void> {
    const file = this.selectedPlugin();
    if (!file || this.isUploadingPlugin()) {
      return;
    }

    this.uploadingPlugin.set(true);
    try {
      const success = await this.scoringService.uploadStrategyPlugin(file);
      if (success) {
        this.feedback.success(`Estrategia '${file.name}' cargada correctamente.`);
        this.selectedPlugin.set(null);
        await this.loadStrategies();
      } else {
        this.feedback.error('No se pudo subir la estrategia. Intenta nuevamente.');
      }
    } finally {
      this.uploadingPlugin.set(false);
    }
  }

  async onLoadRanking(): Promise<void> {
    if (this.rankingForm.invalid) {
      markAllAsTouched(this.rankingForm);
      return;
    }
    const { date } = this.rankingForm.getRawValue();
    if (!date) return;

    this.loadingRanking.set(true);
    try {
      const res = await this.scoringService.getDailyRanking(date);
      this.ranking.set(res?.ranking ?? []);
      if (!res?.ranking?.length) {
        this.feedback.info('No se registraron posiciones para la fecha seleccionada.');
      }
    } finally {
      this.loadingRanking.set(false);
    }
  }

  private today(): string {
    const now = new Date();
    return now.toISOString().substring(0, 10);
  }
}
