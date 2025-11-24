import { Component, inject, signal } from '@angular/core';
import { CommonModule, DatePipe, NgIf } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FeedbackService } from '../services/feedback.service';
import { ReportsService, AttractionUsageReport } from '../services/reports.service';
import { markAllAsTouched } from '../shared/utils/forms.util';

@Component({
  selector: 'app-attraction-usage-report',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NgIf,
    DatePipe,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './attraction-usage.component.html',

})
export class AttractionUsageReportComponent {
  private fb = inject(FormBuilder);
  private reportsService = inject(ReportsService);
  private feedback = inject(FeedbackService);

  private loading = signal(false);
  reports = signal<AttractionUsageReport[]>([]);
  displayedColumns: string[] = ['attraction', 'visits', 'average', 'capacity', 'occupancy', 'peakDay'];

  readonly form = this.fb.group({
    startDate: [this.buildStartDate(), Validators.required],
    endDate: [this.buildEndDate(), Validators.required],
  });

  constructor() {
    void this.onRunReport();
  }

  isLoading = () => this.loading();

  async onRunReport(): Promise<void> {
    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }

    const { startDate, endDate } = this.form.getRawValue();
    if (!startDate || !endDate) return;

    if (new Date(startDate) > new Date(endDate)) {
      this.feedback.error('La fecha inicial no puede ser posterior a la fecha final.');
      return;
    }

    this.loading.set(true);
    try {
      const data = await this.reportsService.getAttractionUsage(startDate, endDate);
      this.reports.set(data);
      if (!data.length) {
        this.feedback.info('No se encontraron registros para el periodo seleccionado.');
      }
    } finally {
      this.loading.set(false);
    }
  }

  private buildEndDate(): string {
    const today = new Date();
    return today.toISOString().substring(0, 10);
  }

  private buildStartDate(): string {
    const date = new Date();
    date.setDate(date.getDate() - 7);
    return date.toISOString().substring(0, 10);
  }
}
