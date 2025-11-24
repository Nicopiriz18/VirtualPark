import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe, NgFor, NgIf } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { AttractionAccessService } from '../services/attraction-access.service';
import { AttractionService } from '../services/attraction.service';
import { FeedbackService } from '../services/feedback.service';
import { Attraction } from '../models/attraction';
import { Incidence } from '../models/incidence';
import { CreateIncidenceRequest } from '../models/create-incidence-request';
import { markAllAsTouched } from '../shared/utils/forms.util';

@Component({
  selector: 'app-incidents',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    NgIf,
    NgFor,
    DatePipe,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatDividerModule,
  ],
  templateUrl: './incidents.component.html',
})
export class IncidentsComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private accessService = inject(AttractionAccessService);
  private attractionService = inject(AttractionService);
  private feedback = inject(FeedbackService);

  private loadingAttractions = signal(false);
  private loadingIncidents = signal(false);
  private submitting = signal(false);

  attractions = signal<Attraction[]>([]);
  incidents = signal<Incidence[]>([]);
  currentAttractionId = signal<string>('');

  selectionForm = this.fb.group({
    attractionId: [''],
  });

  incidentForm = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(120)]],
    description: ['', [Validators.required, Validators.maxLength(1000)]],
    status: [true, Validators.required],
    date: [''],
  });

  isLoadingAttractions = () => this.loadingAttractions();
  isLoadingIncidents = () => this.loadingIncidents();
  isSubmitting = () => this.submitting();

  async ngOnInit(): Promise<void> {
    await this.loadAttractions();
    const preselected = this.route.snapshot.paramMap.get('id') ?? '';
    if (preselected) {
      this.selectionForm.patchValue({ attractionId: preselected });
      await this.onAttractionChange(preselected);
    }
  }

  async refreshAttractions(): Promise<void> {
    await this.loadAttractions();
    this.feedback.success('Listado de atracciones actualizado.');
  }

  async onAttractionChange(attractionId: string): Promise<void> {
    this.selectionForm.patchValue({ attractionId }, { emitEvent: false });
    this.currentAttractionId.set(attractionId);
    if (!attractionId) {
      this.incidents.set([]);
      return;
    }
    await this.loadIncidents(attractionId);
  }

  async onCreateIncident(): Promise<void> {
    if (!this.currentAttractionId()) {
      this.feedback.error('Selecciona una atracción antes de registrar la incidencia.');
      markAllAsTouched(this.selectionForm);
      return;
    }

    if (this.incidentForm.invalid) {
      markAllAsTouched(this.incidentForm);
      return;
    }

    this.submitting.set(true);
    try {
      const request: CreateIncidenceRequest = {
        title: this.incidentForm.value.title?.trim() ?? '',
        description: this.incidentForm.value.description?.trim() ?? '',
        status: this.incidentForm.value.status ?? true,
        date: this.buildIsoDate(this.incidentForm.value.date),
      };

      const created = await this.accessService.createIncident(this.currentAttractionId(), request);
      if (created) {
        this.feedback.success('La incidencia se registró correctamente.');
        this.incidentForm.reset({ status: true, date: '' });
        await this.loadIncidents(this.currentAttractionId());
      } else {
        this.feedback.error('No se pudo registrar la incidencia. Intenta nuevamente.');
      }
    } finally {
      this.submitting.set(false);
    }
  }

  async handleStatusChange(incident: Incidence): Promise<void> {
    if (!this.currentAttractionId()) return;

    const action = incident.status ? this.accessService.closeIncident : this.accessService.reopenIncident;
    const toggleMessage = incident.status
      ? '¿Deseas marcar esta incidencia como resuelta?'
      : '¿Deseas reabrir esta incidencia?';

    if (!confirm(toggleMessage)) {
      return;
    }

    const updated = await action.call(this.accessService, this.currentAttractionId(), incident.id);
    if (updated) {
      this.feedback.success('El estado de la incidencia se actualizó correctamente.');
      await this.loadIncidents(this.currentAttractionId());
    } else {
      this.feedback.error('No se pudo actualizar el estado de la incidencia.');
    }
  }

  goToRegister(): void {
    if (!this.currentAttractionId()) return;
    this.router.navigate(['access'], { queryParams: { attractionId: this.currentAttractionId() } });
  }

  goToCapacity(): void {
    if (!this.currentAttractionId()) return;
    this.router.navigate(['access', 'capacity', this.currentAttractionId()]);
  }

  private async loadAttractions(): Promise<void> {
    this.loadingAttractions.set(true);
    try {
      await this.attractionService.fetchAttractions();
      this.attractions.set(this.attractionService.attractionsSignal());
    } finally {
      this.loadingAttractions.set(false);
    }
  }

  private async loadIncidents(attractionId: string): Promise<void> {
    this.loadingIncidents.set(true);
    try {
      const list = await this.accessService.getIncidents(attractionId);
      this.incidents.set(list ?? []);
    } finally {
      this.loadingIncidents.set(false);
    }
  }

  private buildIsoDate(dateValue: string | null | undefined): string {
    if (!dateValue) {
      return new Date().toISOString();
    }
    const date = new Date(dateValue);
    return Number.isNaN(date.getTime()) ? new Date().toISOString() : date.toISOString();
  }
}
