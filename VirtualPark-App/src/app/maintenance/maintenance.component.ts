import { Component, inject, signal } from '@angular/core';
import { CommonModule, DatePipe, NgIf } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AttractionService } from '../services/attraction.service';
import { FeedbackService } from '../services/feedback.service';
import { MaintenanceDto, MaintenanceService } from '../services/maintenance.service';
import { Attraction } from '../models/attraction';
import { markAllAsTouched } from '../shared/utils/forms.util';

@Component({
  selector: 'app-maintenance',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NgIf,
    DatePipe,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './maintenance.component.html',

})
export class MaintenanceComponent {
  private fb = inject(FormBuilder);
  private attractionService = inject(AttractionService);
  private maintenanceService = inject(MaintenanceService);
  private feedback = inject(FeedbackService);

  private loadingAttractions = signal(false);
  private loadingMaintenances = signal(false);
  private scheduling = signal(false);

  attractions = signal<Attraction[]>([]);
  maintenances = signal<MaintenanceDto[]>([]);
  currentAttractionId = signal<string>('');

  displayedColumns: string[] = ['date', 'description', 'actions'];

  form = this.fb.group({
    attractionId: ['', Validators.required],
    scheduledDate: ['', Validators.required],
    startTime: ['', Validators.required],
    estimatedDuration: ['', Validators.required],
    description: ['', Validators.required],
  });

  constructor() {
    void this.loadAttractions();
  }

  isLoadingAttractions = () => this.loadingAttractions();
  isLoadingMaintenances = () => this.loadingMaintenances();
  isScheduling = () => this.scheduling();

  async loadAttractions(): Promise<void> {
    this.loadingAttractions.set(true);
    try {
      await this.attractionService.fetchAttractions();
      const list = this.attractionService.attractionsSignal();
      this.attractions.set(list);
    } finally {
      this.loadingAttractions.set(false);
    }
  }

  async onAttractionChange(attractionId: string): Promise<void> {
    this.currentAttractionId.set(attractionId ?? '');
    if (!attractionId) {
      this.maintenances.set([]);
      return;
    }
    await this.loadMaintenances(attractionId);
  }

  async onSchedule(): Promise<void> {
    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }
    const { attractionId, scheduledDate, startTime, estimatedDuration, description } = this.form.getRawValue();
    if (!attractionId || !scheduledDate || !startTime || !estimatedDuration || !description) return;

    this.scheduling.set(true);
    try {
      const request = {
        attractionId,
        scheduledDate: new Date(scheduledDate).toISOString(),
        startTime: this.toTimeSpan(startTime),
        estimatedDuration: this.toTimeSpan(estimatedDuration),
        description: description.trim(),
      };
      const created = await this.maintenanceService.schedule(request);
      if (created) {
        this.feedback.success('Mantenimiento programado correctamente.');
        await this.loadMaintenances(attractionId);
        this.resetForm(false);
      } else {
        this.feedback.error('No se pudo programar el mantenimiento. Intenta nuevamente.');
      }
    } finally {
      this.scheduling.set(false);
    }
  }

  async onDelete(item: MaintenanceDto): Promise<void> {
    if (!confirm('¿Deseas cancelar este mantenimiento programado?')) {
      return;
    }
    const success = await this.maintenanceService.delete(item.id);
    if (success) {
      this.feedback.success('Mantenimiento cancelado correctamente.');
      if (this.currentAttractionId()) {
        await this.loadMaintenances(this.currentAttractionId());
      }
    } else {
      this.feedback.error('No se pudo cancelar el mantenimiento.');
    }
  }

  resetForm(clearAttraction = false): void {
    const attractionId = clearAttraction ? '' : this.form.value.attractionId ?? '';
    this.form.reset({
      attractionId,
      scheduledDate: '',
      startTime: '',
      estimatedDuration: '',
      description: '',
    });
  }

  formatTime(value: string): string {
    if (!value) return '';
    const [hours, minutes] = value.split(':');
    return `${hours?.padStart(2, '0') ?? '00'}:${minutes?.padStart(2, '0') ?? '00'} hs`;
  }

  private async loadMaintenances(attractionId: string): Promise<void> {
    this.loadingMaintenances.set(true);
    try {
      const list = await this.maintenanceService.getByAttraction(attractionId);
      this.maintenances.set(list);
    } finally {
      this.loadingMaintenances.set(false);
    }
  }

  private toTimeSpan(value: string): string {
    if (!value) return '00:00:00';
    const parts = value.split(':');
    const hours = parts[0]?.padStart(2, '0') ?? '00';
    const minutes = parts[1]?.padStart(2, '0') ?? '00';
    return `${hours}:${minutes}:00`;
  }
}
